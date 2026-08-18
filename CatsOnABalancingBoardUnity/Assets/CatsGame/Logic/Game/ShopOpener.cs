using System.Collections;
using OMC;
using OMC.ECS;
using UnityEngine;

public class ShopOpener : MonoBehaviour
{
    GameLogicScript gameLogic; 
    public float timeToNextCheck = 17;
    public float thresholdScaler = 0.1f;
    public int shopOnThisManyThresholdsMet = 6;
    long catCountAllTimePrevious;
    long diffThreshold;

    public void Start()
    {
        gameLogic = GameLogicScript.instance;

        WeightDropper.FirstWeightDropped += GetStartedWithIt;
        ShopBehaviour.OnShopClosed += GetStartedWithIt;
    }

    void OnDestroy() 
    {
        WeightDropper.FirstWeightDropped -= GetStartedWithIt;
        ShopBehaviour.OnShopClosed -= GetStartedWithIt;
    }

    void GetStartedWithIt()
    {
        catCountAllTimePrevious = gameLogic.catCountAllTime;
        diffThreshold = 0;
        StartCoroutine(CheckOpenShop());
    }

    int thresholdMetCounter;
    IEnumerator CheckOpenShop()
    {
        thresholdMetCounter = 0;

        while (gameObject)
        {
            yield return new WaitForSeconds(timeToNextCheck);
        
            long diff = gameLogic.catCountAllTime - catCountAllTimePrevious;
            catCountAllTimePrevious = gameLogic.catCountAllTime;

            string debugstring = $"gained since last check {diff}, threshold {diffThreshold}";
            
            if (diff < diffThreshold)
            {
                ShopBehaviour.instance.ActivateShopPrompt();
                
                Debug.Log(debugstring + ", activated shop prompt by threshold failed");
                
                break;
            } 
            else if (thresholdMetCounter == shopOnThisManyThresholdsMet)
            {
                ShopBehaviour.instance.ActivateShopPrompt();

                Debug.Log(debugstring + ", activated shop prompt by meeting enough thresholds");
                
                break;
            } 
            else
            {
                thresholdMetCounter++;

                diffThreshold = (long)Mathf.Ceil(thresholdScaler * gameLogic.catCount);
                
                Debug.Log(debugstring + $", new threshold {diffThreshold}");
            }
            
        }
    }
}
