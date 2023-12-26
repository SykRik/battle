public class AxieDefender : AxieBase
{
    public override void Init(string axieID, string genes, int hp, Node node = null)
    {
        base.Init(axieID, genes, hp, node);
        team = Team.Defender;
    }
}
