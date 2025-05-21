using UnityEngine;

public class FlowerController : MonoBehaviour
{
    //static protected FlowerComboController comboCtrlSTATIC;

    public static HarvestLevelController HarvestLvCtrlSTATIC { private get; set; }

    protected FlowerGenerator flowerGenerator;

    [SerializeField]
    
    protected float scoreGain = 10f;

    [SerializeField]
    
    protected float timerGain = 1f;

    [SerializeField]
    
    protected int resourceIdx = 0;

    [SerializeField]
    
    protected float destroyWaitTime = 2f;

    protected FlowerFXController visual;

    protected void Start()
    {
        visual = GetComponent<FlowerFXController>();
    }

    public void InitFlower(FlowerGenerator generator)
    {
        flowerGenerator = generator;

        if (flowerGenerator != null)
        {
            flowerGenerator.AllFlowers.Add(this);
        }

        StartCoroutine(CoroutineExtensions.DelayedActionCall(ActivateFlower, 0.5f));
    }

    public virtual void AuraAffectFlower(float affection)
    {

    }

    protected void completeBloom()
    {
        DeactivateFlower();

        //Change so that use lv satu ratio
        visual.TriggerHarvestFX(HarvestLvCtrlSTATIC.GetLvSatuRatio());

        //PlaySystemRefStorage.scoreManager.AddBloomScore(resourceIdx, HarvestLvCtrlSTATIC.GetScoreBonus() * scoreGain);

        //PlaySystemRefStorage.playTimeManager.ChangeTimer(HarvestLvCtrlSTATIC.GetTimeBonus() * timerGain);

        HarvestLvCtrlSTATIC.AddExp(0.15f * timerGain);

        if (flowerGenerator != null) 
        { 
            flowerGenerator.AllFlowers.Remove(this);

            flowerGenerator.GatherFlower(); 
        }

        //comboCtrlSTATIC.AddCombo();
    }

    protected virtual void ActivateFlower()
    {
        GetComponent<Collider>().enabled = true;
    }

    protected virtual void DeactivateFlower()
    {
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, destroyWaitTime);
    }
}