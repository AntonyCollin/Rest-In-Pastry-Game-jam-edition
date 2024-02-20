using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectionManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private Image itemImg;
    public void NewInspection(CookingData item, bool isFromBlueGhost)
    {
        itemImg.gameObject.SetActive(true);

        itemImg.sprite = item.image;

        if (isFromBlueGhost)
        {
            CookingData oppositeIngredient = item.oppositeIngredient;

            itemText.text = oppositeIngredient.name;
            itemImg.sprite = oppositeIngredient.image;
        }
        else
        {



            //itemText.text = item.isFinalIngredient() ? item.finalIngredientName : item.name;

            //if (item.isFinalIngredient && (item.finalIngredientName == null || item.finalIngredientName.Length <= 1))
            //{
                itemText.text = item.name;
        }

    }

    public void ClearInspection()
    {
        itemImg.gameObject.SetActive(false);
        itemText.text = string.Empty;
    }
}
