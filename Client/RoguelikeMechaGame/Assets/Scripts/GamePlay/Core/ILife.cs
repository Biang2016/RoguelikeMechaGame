public interface ILife
{
    int M_LeftLife { get; }
    int M_TotalLife { get; }
    void AddLife(int addLifeValue);
    void Heal(int healValue);
    void Damage(int damage);
    void HealAll();
    void Change(int change);
    void ChangeMaxLife(int change);
}