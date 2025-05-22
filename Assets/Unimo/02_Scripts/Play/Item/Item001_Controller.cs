public class Item001_Controller : ItemController
{
    private static PlayTimeManager playTimeManager;

    private readonly float gainRatio = 0.2f;

    public override void UseItem()
    {
        if (playTimeManager == null) { playTimeManager = PlaySystemRefStorage.playTimeManager; }

        playTimeManager.ChangeTimer(gainRatio * playTimeManager.GetMaxTime());

        //Base_Mng.Data.data.TimeItem++;

        base.UseItem();
    }
}