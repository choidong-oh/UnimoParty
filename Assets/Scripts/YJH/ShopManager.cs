using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Button buyButton;
    private int ingameMoney;
    [SerializeField] TextMeshProUGUI money1;

    [Header("몸 프리팹")]
    [SerializeField] GameObject riBeePrefab;
    [SerializeField] GameObject catPrefab;

    [Header("스폰 포지션")]
    [SerializeField] Transform body;
    [SerializeField] Transform spaceShip;

    [Header("스크롤 뷰들")]
    [SerializeField] GameObject[] scrollVeiw;

    [Header("토글 들")]
    [SerializeField] Toggle[] viewToggles;


    private SpaceShipData selectedSpaceShipData;

    private void Start()
    {
        ingameMoney = Manager.Instance.observer.UserPlayer.gamedata._money;
        money1.text = ingameMoney.ToString();



        for (int i = 0; i < viewToggles.Length; i++)
        {
            if (viewToggles[i].isOn)
            {
                ViewActive(i);
                break;
            }
        }
    }

    public void ViewActive(int activeIndex)
    {
        for (int i = 0; i < scrollVeiw.Length; i++)
        {
            scrollVeiw[i].SetActive(i == activeIndex);
        }
    }

    public void OnShipSelect(SpaceShipData selectedShip)
    {        

        selectedSpaceShipData = selectedShip;
        buyButton.gameObject.SetActive(true);
        buyButton.onClick.AddListener(BuyShipButton);
    }

    public void BuyShipButton()
    {
        Manager.Instance.observer.BuyShip(selectedSpaceShipData);


        ingameMoney = Manager.Instance.observer.UserPlayer.gamedata._money;
        money1.text = ingameMoney.ToString();

        FirebaseAuthMgr.Instance.SaveUserData(FirebaseAuthMgr.user.DisplayName, "ShipData", selectedSpaceShipData.Name);
        selectedSpaceShipData = null;
        buyButton.gameObject.SetActive(false);
    }

}
