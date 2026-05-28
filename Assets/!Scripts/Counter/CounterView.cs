public class CounterView
{
    CounterInteractable interactable;
    CounterViewData data;

    public CounterView(CounterInteractable interactable, CounterViewData data)
    {
        this.interactable = interactable;
        this.data = data;
    }

    public void UpdateCounterTimer(string str)
    {   
        this.data.timerText?.SetText(str);
    }

    public void RefreshOrderView(Order order)
    {
        int size = order.Count;
        for(int i = size; i < data.orderImages.Length; i++)
        {
            data.orderImages[i].gameObject.SetActive(false);
        }

        for(int i = 0 ; i < size; i++)
        {
            data.orderImages[i].gameObject.SetActive(true);
            data.orderImages[i].sprite = order.GetOrderItemAt(i).sprite;
        }

        ToggleOrderView(true);
    }

    public void ToggleOrderView(bool isActive)
    {
        data.cgMain.alpha = isActive ? 1.0f : 0.0f;
    }



}
