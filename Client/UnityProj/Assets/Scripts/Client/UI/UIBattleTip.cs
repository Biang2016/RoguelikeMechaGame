using BiangStudio.GamePlay.UI;
using BiangStudio.ObjectPool;
using UnityEngine;

namespace Client
{
    public enum EAttackerType
    {
        None = 0,
        LocalPlayer = 1,
        Enemy = 2,
        Team = 3,

        NoTip = 100,
    }

    public class UIBattleTip : PoolObject
    {
        public float disappearTime = 1.5f;
        float elapseTime = 1.5f;
        UIBattleTipParam attackParam = new UIBattleTipParam();

        public SpriteText3D spriteIcon;
        public SpriteText3D spriteTextType;
        public SpriteText3D spriteTextContext;
        public SpriteText3D spriteTextElementContext;

        public SpriteText3D spriteWhiteIcon;
        public SpriteText3D spriteWhiteTextType;
        public SpriteText3D spriteWhiteTextContext;
        public SpriteText3D spriteWhiteTextElementContext;

        public Sprite3D spriteImage;

        public SpriteText3D[] needPlayAni;
        public Sprite3D[] needSpritePlayAni;


        public Animator animator;

        protected Vector3 contextLocalPos = Vector3.zero;
        protected Vector3 contextElementLocalPos = Vector3.zero;
        protected Vector3 typeLocalPos = Vector3.zero;
        protected Vector3 iconLocalPos = Vector3.zero;


        protected Color color = new Color();
        protected Vector3 offsetPos = new Vector3();
        protected Vector3 scale = new Vector3();

        protected UIBattleTipManager UIBattleTipManager;

        Transform cacheTrans = null;
        Camera mainCamera;
        Camera ui3DCamera;

        Camera GetUI3DCamera
        {
            get
            {
                if (ui3DCamera == null)
                {
                    ui3DCamera = UIManager.Instance.UICamera;
                }

                return ui3DCamera;
            }
        }

        Camera GetMainCamera
        {
            get
            {
                if (mainCamera == null)
                {
                    mainCamera = CameraManager.Instance.MainCamera;
                }

                return mainCamera;
            }
        }

        void Awake()
        {
            if (spriteTextContext != null)
            {
                contextLocalPos = spriteTextContext.transform.localPosition;
            }

            if (spriteTextElementContext != null)
            {
                contextElementLocalPos = spriteTextElementContext.transform.localPosition;
            }

            if (spriteTextType != null)
            {
                typeLocalPos = spriteTextType.transform.localPosition;
            }

            if (spriteIcon != null)
            {
                iconLocalPos = spriteIcon.transform.localPosition;
            }

            cacheTrans = transform;
        }

        public void Setup(UIBattleTipParam param, UIBattleTipManager battleTipManager)
        {
            UIBattleTipManager = battleTipManager;

            param.CopyData(attackParam);
            elapseTime = disappearTime;
            InitTip();
        }

        bool UpdateSpriteByText(SpriteText3D sprite, string note, ref Vector3 pos, int page = -1, bool chagneClr = true)
        {
            bool result = false;
            if (sprite != null)
            {
                if (page >= 0)
                    sprite.Page = page;
                if (note != null && note.Length > 0)
                {
                    sprite.text = note;
                    if (chagneClr)
                    {
                        sprite.color = color;
                    }
                    else
                    {
                        sprite.color = Color.white;
                    }

                    sprite.gameObject.CustomSetActive(true);
                    sprite.transform.localPosition = pos + offsetPos;
                    result = true;
                }
                else
                {
                    sprite.gameObject.CustomSetActive(false);
                }
            }

            return result;
        }

        void SetContextSprite(SpriteText3D sprite, long diffHP, bool hadType, int page = -1, bool chagneClr = true)
        {
            if (sprite != null)
            {
                if (page >= 0)
                    sprite.Page = page;
                sprite.text = diffHP.ToString();

                if (chagneClr)
                {
                    sprite.color = color;
                }
                else
                {
                    sprite.color = Color.white;
                }

                //spriteTextContext.transform.localPosition = (hadType ? typeLocalPos : contextLocalPos) + offsetPos;
                var pos = (hadType ? contextLocalPos : typeLocalPos) + offsetPos;
                sprite.transform.SetLocalPosition(pos.x, pos.y, pos.z);
            }
        }

        void SetContextElementSprite(SpriteText3D sprite, long diffHP, bool hadType, int page = -1, bool chagneClr = true)
        {
            if (sprite != null)
            {
                if (page >= 0)
                    sprite.Page = page;
                if (diffHP == 0)
                {
                    sprite.gameObject.CustomSetActive(false);
                }
                else
                {
                    sprite.gameObject.CustomSetActive(true);
                    sprite.text = diffHP.ToString();
                }

                if (chagneClr)
                {
                    sprite.color = color;
                }
                else
                {
                    sprite.color = Color.white;
                }

                var pos = (hadType ? contextElementLocalPos : typeLocalPos) + offsetPos;
                sprite.transform.SetLocalPosition(pos.x, pos.y, pos.z);
            }
        }

        int RandomSquare(int val)
        {
            return Random.Range(-val, val);
        }

        void RandomCircle(int radius, ref Vector3 lastOffset)
        {
            float quadRadius = attackParam.cfg.SquareX * 0.001f;
            float square = quadRadius;
            for (int i = 0; i < 100; ++i)
            {
                int val = Random.Range(-radius, radius);
                float diffX = lastOffset.x + val * 0.001f;
                if (diffX < offsetPos.x + square && diffX > offsetPos.x - square)
                {
                    float diffY = lastOffset.y + val * 0.001f;
                    if (diffY < offsetPos.y + square && diffY > offsetPos.y - square)
                    {
                        offsetPos.x = lastOffset.x + val * 0.001f;
                        offsetPos.y = lastOffset.y + val * 0.001f;
                        //Logger.Log("tip:RandomCircle:" + offsetPos.ToString());
                        return;
                    }
                }
            }

            offsetPos = lastOffset;
        }

        void RandomPos()
        {
            if (UIBattleTipManager == null)
                return;
            TipInfo tipInfo = UIBattleTipManager.GetTipInfo(attackParam.hitIdent, attackParam.attackType);
            if (tipInfo == null)
            {
                int offset = RandomSquare((int) attackParam.cfg.SquareX);
                offsetPos.x += offset * 0.001f;
                offsetPos.y += offset * 0.001f;
                //Logger.Log("tip:RandomSquare:" + offsetPos.ToString());
                UIBattleTipManager.AddTipInfo(attackParam.hitIdent, attackParam.attackType, ref offsetPos);
            }
            else
            {
                RandomCircle((int) attackParam.cfg.Radius, ref tipInfo.lastOffset);
                tipInfo.SetInfo(tipInfo.ident, tipInfo.attackType, ref offsetPos);
            }
        }

        void InitTip()
        {
            if (attackParam != null && attackParam.IsValid())
            {
                float scaleVal = attackParam.cfg.Scale * 0.01f * attackParam.scale;
                scale.Set(scaleVal, scaleVal, scaleVal);
                //Logger.Log(string.Format("InitTip:sacle:{0},sacle:{1}",transform.localScale,scale));

                transform.localScale = scale;

                // Logger.Log(string.Format("scale:{0},cfgScale:{1},calcScale:{2},attacker:{3}",scale, attackParam.cfg.Scale, attackParam.scale, attackParam.acttackerType));
                color.r = attackParam.cfg.ColorRed / 255.0f;
                color.g = attackParam.cfg.ColorGreen / 255.0f;
                color.b = attackParam.cfg.ColorBlue / 255.0f;
                color.a = attackParam.cfg.ColorAlpha / 255.0f;
                offsetPos.x = attackParam.cfg.OffsetX * 0.001f + attackParam.offsetX;
                offsetPos.y = attackParam.cfg.OffsetY * 0.001f + attackParam.offsetY;
                offsetPos.z = attackParam.cfg.OffsetZ * 0.001f;
                if (attackParam.cfg.SquareX > 0)
                {
                    RandomPos();
                }
                else
                {
                    //Logger.Log("tip:normal:" + offsetPos.ToString());
                }

                UpdatePos();
                //transform.localPosition = attackParam.startPos;

                int page = (int) attackParam.cfg.PageNum;
                bool changeClr = attackParam.acttackerType != (uint) EAttackerType.LocalPlayer;

                bool hadType = UpdateSpriteByText(spriteTextType, attackParam.cfg.Note, ref typeLocalPos, page, changeClr);

                //if (attackParam.cfg.Note != "A" && attackParam.cfg.Note != "B" && attackParam.cfg.Note != "C")
                UpdateSpriteByText(spriteIcon, attackParam.cfg.PreNote, ref iconLocalPos, page, changeClr);

                SetContextSprite(spriteTextContext, attackParam.diffHP, hadType, page, changeClr);
                SetContextElementSprite(spriteTextElementContext, attackParam.elementHP, hadType, page, changeClr);

                UpdateSpriteByText(spriteWhiteTextType, attackParam.cfg.Note, ref typeLocalPos);

                //if (attackParam.cfg.Note != "A" && attackParam.cfg.Note != "B" && attackParam.cfg.Note != "C")
                UpdateSpriteByText(spriteWhiteIcon, attackParam.cfg.PreNote, ref iconLocalPos);

                SetContextSprite(spriteWhiteTextContext, attackParam.diffHP, hadType);
                SetContextElementSprite(spriteWhiteTextElementContext, attackParam.elementHP, hadType);

                ResetspriteImageIcon();


                PlayAnim();
            }
        }

        void ResetspriteImageIcon()
        {
            if (spriteImage != null && !string.IsNullOrEmpty(attackParam.spriteImagePath))
            {
                ResourceManager.LoadAsset<Texture2D>(attackParam.spriteImagePath).Completed += LoadIconCallBack;
            }
        }

        void LoadIconCallBack(IAsyncOperation<Texture2D> async)
        {
            if (async == null)
                return;
            if (spriteImage != null)
            {
                spriteImage.texture = async.Result;
            }
            else
            {
                ResourceManager.ReleaseAsset(async.Result);
            }

            async.Release();
        }


        void SetPageNum(SpriteText3D text3D, int page)
        {
            if (text3D != null)
            {
                text3D.Page = page;
            }
        }

        public void Shutdown()
        {
        }

        public void Reset()
        {
            StopAnim();
        }

        void UpdatePos()
        {
            if (GetMainCamera != null && GetUI3DCamera != null && attackParam != null)
            {
                if (attackParam != null && !attackParam.inScreenCenter)
                {
                    var pos = GetMainCamera.WorldToScreenPoint(attackParam.startPos);
                    cacheTrans.localPosition = GetUI3DCamera.ScreenToWorldPoint(pos);
                }
                else
                {
                    cacheTrans.localPosition = Vector3.zero;
                }
            }
        }

        public bool Tick(float detlaTime)
        {
            elapseTime -= detlaTime;
            if (elapseTime < 0.0f)
            {
                Reset();
                return true;
            }

            UpdatePos();
            return false;
        }

        public bool IsEqualParam(UIBattleTipParam param)
        {
            if (param == null)
                return false;
            return param.IsEqual(attackParam);
        }

        void PlayAnim()
        {
            gameObject.CustomSetActive(true);
            //if (animator != null)
            //{
            //    animator.enabled = true;
            //}
            if (needPlayAni != null)
            {
                for (int i = 0; i < needPlayAni.Length; ++i)
                {
                    if (needPlayAni[i] != null)
                    {
                        needPlayAni[i].PlayAnim(null);
                    }
                }
            }

            if (needSpritePlayAni != null)
            {
                for (int i = 0; i < needSpritePlayAni.Length; ++i)
                {
                    if (needSpritePlayAni[i] != null)
                    {
                        needSpritePlayAni[i].PlayAnim(null);
                    }
                }
            }
        }

        void StopAnim()
        {
            gameObject.CustomSetActive(false);

            //if (animator != null)
            //{
            //    animator.enabled = false;
            //}
        }
    }
}