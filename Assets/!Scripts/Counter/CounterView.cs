// updates the counter ui
public class CounterView
{
    CounterInteractable interactable;
    CounterViewData data;

    public CounterView(CounterInteractable interactable, CounterViewData data)
    {
        // keeps the interactable and view data together
        this.interactable = interactable;
        this.data = data;
    }

    public void UpdateCounterTimer(string str)
    {   
        // show the current order timer
        this.data.timerText?.SetText(str);
    }

    public void RefreshOrderView(Order order)
    {
        // mirror the current order into the ui
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
        // show or hide the counter panel
        data.cgMain.alpha = isActive ? 1.0f : 0.0f;
    }



}
