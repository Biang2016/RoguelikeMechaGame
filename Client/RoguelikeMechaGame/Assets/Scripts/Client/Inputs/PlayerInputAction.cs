// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Client
{
    public class @PlayerInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""MechaBattleInput"",
            ""id"": ""1ece4267-24e1-4d81-a6b1-405a9d7c51cb"",
            ""actions"": [
                {
                    ""name"": ""MouseLeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""5a64d0f6-9a46-49fe-834c-30f69d7569b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""MouseRightClick"",
                    ""type"": ""Button"",
                    ""id"": ""78be1988-5a9e-4d77-a087-54214b8a8a85"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""MouseMiddleClick"",
                    ""type"": ""Button"",
                    ""id"": ""dcdf1bc6-6254-491a-874a-b162a7d558d8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""bd466e74-41ff-4ec2-9bc2-c86fdc792344"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skill_0"",
                    ""type"": ""Button"",
                    ""id"": ""10804770-1f15-4fa3-ab77-f9c65eda1450"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Skill_1"",
                    ""type"": ""Button"",
                    ""id"": ""d54c8a7b-ae23-43b7-b6a4-ea80959376ec"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Skill_2"",
                    ""type"": ""Button"",
                    ""id"": ""797408c5-59fe-4154-b3ea-9afca0591bb8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Skill_3"",
                    ""type"": ""Button"",
                    ""id"": ""f027c168-eca3-48e2-be31-6edf44e526b2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e4a4021e-82f8-43f0-885a-287d2c76bf59"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Skill_0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6e531402-3a0b-4fe2-a31f-1bfe037f8fb8"",
                    ""path"": ""<XInputController>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill_0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7772607d-1167-445b-92d2-25c88f0895f1"",
                    ""path"": ""<DualShockGamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill_0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""21be93bb-0e5c-4a59-b6f3-9b516c74bedb"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Skill_1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4f494aa8-d790-488f-92ed-eb872ff00203"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill_1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e433f5f4-1da6-4811-8a30-9779dcf2f46a"",
                    ""path"": ""<DualShockGamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill_1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da13915c-fa95-4a85-9caf-88322ccb72bf"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Skill_2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d159f5b1-f423-44d0-8b13-93a264559f9b"",
                    ""path"": ""<XInputController>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill_2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b16342ce-bb49-4a47-93ec-c85b88ac524e"",
                    ""path"": ""<DualShockGamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill_2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6bae345-756c-4b79-9f20-2a60951a487a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Skill_3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a5f6a9ce-ecd9-4c21-b3ba-78b1d1497938"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill_3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ed93d23b-9510-4320-85f3-98f8c1377c0f"",
                    ""path"": ""<DualShockGamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill_3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1badfae2-bf1a-445a-bfee-f4de44f3323c"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseMiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""637e2b3c-750a-46c5-b9fd-c4a1cc798878"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseLeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""772f6319-2147-4b31-9912-a5af4efc436d"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseRightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aa9bcc24-85ce-491a-ad30-60acc3a6e5b5"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""6b95673d-8a12-412a-9390-8184253550b8"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b0ca1bb8-34d0-457b-8918-390c38c82bf3"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5faf8d5b-04de-42a3-80e9-df1147142717"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e9718a4c-1f96-49cf-8f1f-0f1f9488a89f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5e2988c1-cf9c-4a5c-aaeb-3d177349c4e8"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""MechaBuildingInput"",
            ""id"": ""e3b1181f-7121-4f94-89e7-addd0d7b9803"",
            ""actions"": [
                {
                    ""name"": ""MouseLeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""015b758b-31e4-4e14-ab9d-383d30922006"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""MouseRightClick"",
                    ""type"": ""Button"",
                    ""id"": ""dfc6415d-0fa1-4f02-897c-0a00d4f2c106"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""MouseMiddleClick"",
                    ""type"": ""Button"",
                    ""id"": ""91a6bbaa-0bcc-4eb3-b451-59fa66970d2d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""RotateItem"",
                    ""type"": ""Button"",
                    ""id"": ""097c04ba-5832-435d-8a3d-368f89c5f76b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""ToggleWireLines"",
                    ""type"": ""Button"",
                    ""id"": ""ab7dc26b-d210-49a5-bc1e-93e822716dbc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""ToggleBag"",
                    ""type"": ""Button"",
                    ""id"": ""9c4bb76c-4707-4e5c-a7f3-74e9d4fe4aa9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""631bf7b5-0411-4e1c-a8bb-042821cc53d0"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseLeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55efd318-f9f9-4475-b5a8-b4074461a8a9"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""RotateItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""18410dfb-989a-4efe-b6ca-8529847e146b"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""ToggleWireLines"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f36765b4-70ff-4ede-b1f7-71db8bd78fd1"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""ToggleBag"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b23b49ab-92c7-4c71-9d25-26e508838519"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseRightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f46ccae-7a7e-4634-b2f9-1e1afb241b88"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseMiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Common"",
            ""id"": ""7e89e9d1-f7e1-49a8-bcc4-018eff99152e"",
            ""actions"": [
                {
                    ""name"": ""MouseLeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""1c84be6d-be71-459a-b015-48d29931e2a2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""MouseRightClick"",
                    ""type"": ""Button"",
                    ""id"": ""01504029-9327-4d64-ae1b-ea8d51e2a146"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""MouseMiddleClick"",
                    ""type"": ""Button"",
                    ""id"": ""d3ecfc04-fec2-4614-8ff5-618873f00b28"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Exit"",
                    ""type"": ""Button"",
                    ""id"": ""c221a3f9-4221-4da1-b188-bf04e006da04"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Tab"",
                    ""type"": ""Button"",
                    ""id"": ""21fa8e64-6108-4648-bbba-0381934760dd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Confirm"",
                    ""type"": ""Button"",
                    ""id"": ""9c95a13c-9036-4cae-9675-0ddef4b52926"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Debug"",
                    ""type"": ""Button"",
                    ""id"": ""a3d49bae-abc7-4e11-8476-9ad2ac9ac463"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c2a2ef1b-7b38-4f7c-aabe-258fb695fb5b"",
                    ""path"": ""*/{Cancel}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;KeyboardMouse"",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be9919a6-8c46-44ff-a0b2-449817606f7b"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": ""Clamp(max=1)"",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Tab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dd493237-8c65-4d8b-961e-f9f1a409bb06"",
                    ""path"": ""*/{Submit}"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Gamepad"",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3eed784b-c75c-4d15-8ec8-e96895d015b0"",
                    ""path"": ""<Keyboard>/BackQuote"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Debug"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1a5710a-99a2-4033-8f47-46dab9355f0a"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseMiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""15e73d6e-7d92-42fa-afe9-d3536d156ea4"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseRightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9857762b-435f-433c-8c10-5693573aa119"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseLeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": []
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": []
        }
    ]
}");
            // MechaBattleInput
            m_MechaBattleInput = asset.FindActionMap("MechaBattleInput", throwIfNotFound: true);
            m_MechaBattleInput_MouseLeftClick = m_MechaBattleInput.FindAction("MouseLeftClick", throwIfNotFound: true);
            m_MechaBattleInput_MouseRightClick = m_MechaBattleInput.FindAction("MouseRightClick", throwIfNotFound: true);
            m_MechaBattleInput_MouseMiddleClick = m_MechaBattleInput.FindAction("MouseMiddleClick", throwIfNotFound: true);
            m_MechaBattleInput_Move = m_MechaBattleInput.FindAction("Move", throwIfNotFound: true);
            m_MechaBattleInput_Skill_0 = m_MechaBattleInput.FindAction("Skill_0", throwIfNotFound: true);
            m_MechaBattleInput_Skill_1 = m_MechaBattleInput.FindAction("Skill_1", throwIfNotFound: true);
            m_MechaBattleInput_Skill_2 = m_MechaBattleInput.FindAction("Skill_2", throwIfNotFound: true);
            m_MechaBattleInput_Skill_3 = m_MechaBattleInput.FindAction("Skill_3", throwIfNotFound: true);
            // MechaBuildingInput
            m_MechaBuildingInput = asset.FindActionMap("MechaBuildingInput", throwIfNotFound: true);
            m_MechaBuildingInput_MouseLeftClick = m_MechaBuildingInput.FindAction("MouseLeftClick", throwIfNotFound: true);
            m_MechaBuildingInput_MouseRightClick = m_MechaBuildingInput.FindAction("MouseRightClick", throwIfNotFound: true);
            m_MechaBuildingInput_MouseMiddleClick = m_MechaBuildingInput.FindAction("MouseMiddleClick", throwIfNotFound: true);
            m_MechaBuildingInput_RotateItem = m_MechaBuildingInput.FindAction("RotateItem", throwIfNotFound: true);
            m_MechaBuildingInput_ToggleWireLines = m_MechaBuildingInput.FindAction("ToggleWireLines", throwIfNotFound: true);
            m_MechaBuildingInput_ToggleBag = m_MechaBuildingInput.FindAction("ToggleBag", throwIfNotFound: true);
            // Common
            m_Common = asset.FindActionMap("Common", throwIfNotFound: true);
            m_Common_MouseLeftClick = m_Common.FindAction("MouseLeftClick", throwIfNotFound: true);
            m_Common_MouseRightClick = m_Common.FindAction("MouseRightClick", throwIfNotFound: true);
            m_Common_MouseMiddleClick = m_Common.FindAction("MouseMiddleClick", throwIfNotFound: true);
            m_Common_Exit = m_Common.FindAction("Exit", throwIfNotFound: true);
            m_Common_Tab = m_Common.FindAction("Tab", throwIfNotFound: true);
            m_Common_Confirm = m_Common.FindAction("Confirm", throwIfNotFound: true);
            m_Common_Debug = m_Common.FindAction("Debug", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // MechaBattleInput
        private readonly InputActionMap m_MechaBattleInput;
        private IMechaBattleInputActions m_MechaBattleInputActionsCallbackInterface;
        private readonly InputAction m_MechaBattleInput_MouseLeftClick;
        private readonly InputAction m_MechaBattleInput_MouseRightClick;
        private readonly InputAction m_MechaBattleInput_MouseMiddleClick;
        private readonly InputAction m_MechaBattleInput_Move;
        private readonly InputAction m_MechaBattleInput_Skill_0;
        private readonly InputAction m_MechaBattleInput_Skill_1;
        private readonly InputAction m_MechaBattleInput_Skill_2;
        private readonly InputAction m_MechaBattleInput_Skill_3;
        public struct MechaBattleInputActions
        {
            private @PlayerInput m_Wrapper;
            public MechaBattleInputActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @MouseLeftClick => m_Wrapper.m_MechaBattleInput_MouseLeftClick;
            public InputAction @MouseRightClick => m_Wrapper.m_MechaBattleInput_MouseRightClick;
            public InputAction @MouseMiddleClick => m_Wrapper.m_MechaBattleInput_MouseMiddleClick;
            public InputAction @Move => m_Wrapper.m_MechaBattleInput_Move;
            public InputAction @Skill_0 => m_Wrapper.m_MechaBattleInput_Skill_0;
            public InputAction @Skill_1 => m_Wrapper.m_MechaBattleInput_Skill_1;
            public InputAction @Skill_2 => m_Wrapper.m_MechaBattleInput_Skill_2;
            public InputAction @Skill_3 => m_Wrapper.m_MechaBattleInput_Skill_3;
            public InputActionMap Get() { return m_Wrapper.m_MechaBattleInput; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MechaBattleInputActions set) { return set.Get(); }
            public void SetCallbacks(IMechaBattleInputActions instance)
            {
                if (m_Wrapper.m_MechaBattleInputActionsCallbackInterface != null)
                {
                    @MouseLeftClick.started -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMouseLeftClick;
                    @MouseLeftClick.performed -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMouseLeftClick;
                    @MouseLeftClick.canceled -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMouseLeftClick;
                    @MouseRightClick.started -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMouseRightClick;
                    @MouseRightClick.performed -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMouseRightClick;
                    @MouseRightClick.canceled -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMouseRightClick;
                    @MouseMiddleClick.started -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMouseMiddleClick;
                    @MouseMiddleClick.performed -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMouseMiddleClick;
                    @MouseMiddleClick.canceled -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMouseMiddleClick;
                    @Move.started -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnMove;
                    @Skill_0.started -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_0;
                    @Skill_0.performed -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_0;
                    @Skill_0.canceled -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_0;
                    @Skill_1.started -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_1;
                    @Skill_1.performed -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_1;
                    @Skill_1.canceled -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_1;
                    @Skill_2.started -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_2;
                    @Skill_2.performed -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_2;
                    @Skill_2.canceled -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_2;
                    @Skill_3.started -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_3;
                    @Skill_3.performed -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_3;
                    @Skill_3.canceled -= m_Wrapper.m_MechaBattleInputActionsCallbackInterface.OnSkill_3;
                }
                m_Wrapper.m_MechaBattleInputActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MouseLeftClick.started += instance.OnMouseLeftClick;
                    @MouseLeftClick.performed += instance.OnMouseLeftClick;
                    @MouseLeftClick.canceled += instance.OnMouseLeftClick;
                    @MouseRightClick.started += instance.OnMouseRightClick;
                    @MouseRightClick.performed += instance.OnMouseRightClick;
                    @MouseRightClick.canceled += instance.OnMouseRightClick;
                    @MouseMiddleClick.started += instance.OnMouseMiddleClick;
                    @MouseMiddleClick.performed += instance.OnMouseMiddleClick;
                    @MouseMiddleClick.canceled += instance.OnMouseMiddleClick;
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Skill_0.started += instance.OnSkill_0;
                    @Skill_0.performed += instance.OnSkill_0;
                    @Skill_0.canceled += instance.OnSkill_0;
                    @Skill_1.started += instance.OnSkill_1;
                    @Skill_1.performed += instance.OnSkill_1;
                    @Skill_1.canceled += instance.OnSkill_1;
                    @Skill_2.started += instance.OnSkill_2;
                    @Skill_2.performed += instance.OnSkill_2;
                    @Skill_2.canceled += instance.OnSkill_2;
                    @Skill_3.started += instance.OnSkill_3;
                    @Skill_3.performed += instance.OnSkill_3;
                    @Skill_3.canceled += instance.OnSkill_3;
                }
            }
        }
        public MechaBattleInputActions @MechaBattleInput => new MechaBattleInputActions(this);

        // MechaBuildingInput
        private readonly InputActionMap m_MechaBuildingInput;
        private IMechaBuildingInputActions m_MechaBuildingInputActionsCallbackInterface;
        private readonly InputAction m_MechaBuildingInput_MouseLeftClick;
        private readonly InputAction m_MechaBuildingInput_MouseRightClick;
        private readonly InputAction m_MechaBuildingInput_MouseMiddleClick;
        private readonly InputAction m_MechaBuildingInput_RotateItem;
        private readonly InputAction m_MechaBuildingInput_ToggleWireLines;
        private readonly InputAction m_MechaBuildingInput_ToggleBag;
        public struct MechaBuildingInputActions
        {
            private @PlayerInput m_Wrapper;
            public MechaBuildingInputActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @MouseLeftClick => m_Wrapper.m_MechaBuildingInput_MouseLeftClick;
            public InputAction @MouseRightClick => m_Wrapper.m_MechaBuildingInput_MouseRightClick;
            public InputAction @MouseMiddleClick => m_Wrapper.m_MechaBuildingInput_MouseMiddleClick;
            public InputAction @RotateItem => m_Wrapper.m_MechaBuildingInput_RotateItem;
            public InputAction @ToggleWireLines => m_Wrapper.m_MechaBuildingInput_ToggleWireLines;
            public InputAction @ToggleBag => m_Wrapper.m_MechaBuildingInput_ToggleBag;
            public InputActionMap Get() { return m_Wrapper.m_MechaBuildingInput; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MechaBuildingInputActions set) { return set.Get(); }
            public void SetCallbacks(IMechaBuildingInputActions instance)
            {
                if (m_Wrapper.m_MechaBuildingInputActionsCallbackInterface != null)
                {
                    @MouseLeftClick.started -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnMouseLeftClick;
                    @MouseLeftClick.performed -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnMouseLeftClick;
                    @MouseLeftClick.canceled -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnMouseLeftClick;
                    @MouseRightClick.started -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnMouseRightClick;
                    @MouseRightClick.performed -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnMouseRightClick;
                    @MouseRightClick.canceled -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnMouseRightClick;
                    @MouseMiddleClick.started -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnMouseMiddleClick;
                    @MouseMiddleClick.performed -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnMouseMiddleClick;
                    @MouseMiddleClick.canceled -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnMouseMiddleClick;
                    @RotateItem.started -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnRotateItem;
                    @RotateItem.performed -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnRotateItem;
                    @RotateItem.canceled -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnRotateItem;
                    @ToggleWireLines.started -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnToggleWireLines;
                    @ToggleWireLines.performed -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnToggleWireLines;
                    @ToggleWireLines.canceled -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnToggleWireLines;
                    @ToggleBag.started -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnToggleBag;
                    @ToggleBag.performed -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnToggleBag;
                    @ToggleBag.canceled -= m_Wrapper.m_MechaBuildingInputActionsCallbackInterface.OnToggleBag;
                }
                m_Wrapper.m_MechaBuildingInputActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MouseLeftClick.started += instance.OnMouseLeftClick;
                    @MouseLeftClick.performed += instance.OnMouseLeftClick;
                    @MouseLeftClick.canceled += instance.OnMouseLeftClick;
                    @MouseRightClick.started += instance.OnMouseRightClick;
                    @MouseRightClick.performed += instance.OnMouseRightClick;
                    @MouseRightClick.canceled += instance.OnMouseRightClick;
                    @MouseMiddleClick.started += instance.OnMouseMiddleClick;
                    @MouseMiddleClick.performed += instance.OnMouseMiddleClick;
                    @MouseMiddleClick.canceled += instance.OnMouseMiddleClick;
                    @RotateItem.started += instance.OnRotateItem;
                    @RotateItem.performed += instance.OnRotateItem;
                    @RotateItem.canceled += instance.OnRotateItem;
                    @ToggleWireLines.started += instance.OnToggleWireLines;
                    @ToggleWireLines.performed += instance.OnToggleWireLines;
                    @ToggleWireLines.canceled += instance.OnToggleWireLines;
                    @ToggleBag.started += instance.OnToggleBag;
                    @ToggleBag.performed += instance.OnToggleBag;
                    @ToggleBag.canceled += instance.OnToggleBag;
                }
            }
        }
        public MechaBuildingInputActions @MechaBuildingInput => new MechaBuildingInputActions(this);

        // Common
        private readonly InputActionMap m_Common;
        private ICommonActions m_CommonActionsCallbackInterface;
        private readonly InputAction m_Common_MouseLeftClick;
        private readonly InputAction m_Common_MouseRightClick;
        private readonly InputAction m_Common_MouseMiddleClick;
        private readonly InputAction m_Common_Exit;
        private readonly InputAction m_Common_Tab;
        private readonly InputAction m_Common_Confirm;
        private readonly InputAction m_Common_Debug;
        public struct CommonActions
        {
            private @PlayerInput m_Wrapper;
            public CommonActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @MouseLeftClick => m_Wrapper.m_Common_MouseLeftClick;
            public InputAction @MouseRightClick => m_Wrapper.m_Common_MouseRightClick;
            public InputAction @MouseMiddleClick => m_Wrapper.m_Common_MouseMiddleClick;
            public InputAction @Exit => m_Wrapper.m_Common_Exit;
            public InputAction @Tab => m_Wrapper.m_Common_Tab;
            public InputAction @Confirm => m_Wrapper.m_Common_Confirm;
            public InputAction @Debug => m_Wrapper.m_Common_Debug;
            public InputActionMap Get() { return m_Wrapper.m_Common; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CommonActions set) { return set.Get(); }
            public void SetCallbacks(ICommonActions instance)
            {
                if (m_Wrapper.m_CommonActionsCallbackInterface != null)
                {
                    @MouseLeftClick.started -= m_Wrapper.m_CommonActionsCallbackInterface.OnMouseLeftClick;
                    @MouseLeftClick.performed -= m_Wrapper.m_CommonActionsCallbackInterface.OnMouseLeftClick;
                    @MouseLeftClick.canceled -= m_Wrapper.m_CommonActionsCallbackInterface.OnMouseLeftClick;
                    @MouseRightClick.started -= m_Wrapper.m_CommonActionsCallbackInterface.OnMouseRightClick;
                    @MouseRightClick.performed -= m_Wrapper.m_CommonActionsCallbackInterface.OnMouseRightClick;
                    @MouseRightClick.canceled -= m_Wrapper.m_CommonActionsCallbackInterface.OnMouseRightClick;
                    @MouseMiddleClick.started -= m_Wrapper.m_CommonActionsCallbackInterface.OnMouseMiddleClick;
                    @MouseMiddleClick.performed -= m_Wrapper.m_CommonActionsCallbackInterface.OnMouseMiddleClick;
                    @MouseMiddleClick.canceled -= m_Wrapper.m_CommonActionsCallbackInterface.OnMouseMiddleClick;
                    @Exit.started -= m_Wrapper.m_CommonActionsCallbackInterface.OnExit;
                    @Exit.performed -= m_Wrapper.m_CommonActionsCallbackInterface.OnExit;
                    @Exit.canceled -= m_Wrapper.m_CommonActionsCallbackInterface.OnExit;
                    @Tab.started -= m_Wrapper.m_CommonActionsCallbackInterface.OnTab;
                    @Tab.performed -= m_Wrapper.m_CommonActionsCallbackInterface.OnTab;
                    @Tab.canceled -= m_Wrapper.m_CommonActionsCallbackInterface.OnTab;
                    @Confirm.started -= m_Wrapper.m_CommonActionsCallbackInterface.OnConfirm;
                    @Confirm.performed -= m_Wrapper.m_CommonActionsCallbackInterface.OnConfirm;
                    @Confirm.canceled -= m_Wrapper.m_CommonActionsCallbackInterface.OnConfirm;
                    @Debug.started -= m_Wrapper.m_CommonActionsCallbackInterface.OnDebug;
                    @Debug.performed -= m_Wrapper.m_CommonActionsCallbackInterface.OnDebug;
                    @Debug.canceled -= m_Wrapper.m_CommonActionsCallbackInterface.OnDebug;
                }
                m_Wrapper.m_CommonActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MouseLeftClick.started += instance.OnMouseLeftClick;
                    @MouseLeftClick.performed += instance.OnMouseLeftClick;
                    @MouseLeftClick.canceled += instance.OnMouseLeftClick;
                    @MouseRightClick.started += instance.OnMouseRightClick;
                    @MouseRightClick.performed += instance.OnMouseRightClick;
                    @MouseRightClick.canceled += instance.OnMouseRightClick;
                    @MouseMiddleClick.started += instance.OnMouseMiddleClick;
                    @MouseMiddleClick.performed += instance.OnMouseMiddleClick;
                    @MouseMiddleClick.canceled += instance.OnMouseMiddleClick;
                    @Exit.started += instance.OnExit;
                    @Exit.performed += instance.OnExit;
                    @Exit.canceled += instance.OnExit;
                    @Tab.started += instance.OnTab;
                    @Tab.performed += instance.OnTab;
                    @Tab.canceled += instance.OnTab;
                    @Confirm.started += instance.OnConfirm;
                    @Confirm.performed += instance.OnConfirm;
                    @Confirm.canceled += instance.OnConfirm;
                    @Debug.started += instance.OnDebug;
                    @Debug.performed += instance.OnDebug;
                    @Debug.canceled += instance.OnDebug;
                }
            }
        }
        public CommonActions @Common => new CommonActions(this);
        private int m_KeyboardMouseSchemeIndex = -1;
        public InputControlScheme KeyboardMouseScheme
        {
            get
            {
                if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
                return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
            }
        }
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        public interface IMechaBattleInputActions
        {
            void OnMouseLeftClick(InputAction.CallbackContext context);
            void OnMouseRightClick(InputAction.CallbackContext context);
            void OnMouseMiddleClick(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
            void OnSkill_0(InputAction.CallbackContext context);
            void OnSkill_1(InputAction.CallbackContext context);
            void OnSkill_2(InputAction.CallbackContext context);
            void OnSkill_3(InputAction.CallbackContext context);
        }
        public interface IMechaBuildingInputActions
        {
            void OnMouseLeftClick(InputAction.CallbackContext context);
            void OnMouseRightClick(InputAction.CallbackContext context);
            void OnMouseMiddleClick(InputAction.CallbackContext context);
            void OnRotateItem(InputAction.CallbackContext context);
            void OnToggleWireLines(InputAction.CallbackContext context);
            void OnToggleBag(InputAction.CallbackContext context);
        }
        public interface ICommonActions
        {
            void OnMouseLeftClick(InputAction.CallbackContext context);
            void OnMouseRightClick(InputAction.CallbackContext context);
            void OnMouseMiddleClick(InputAction.CallbackContext context);
            void OnExit(InputAction.CallbackContext context);
            void OnTab(InputAction.CallbackContext context);
            void OnConfirm(InputAction.CallbackContext context);
            void OnDebug(InputAction.CallbackContext context);
        }
    }
}
