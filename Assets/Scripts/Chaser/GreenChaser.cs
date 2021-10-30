public class GreenChaser : Chaser
{
    private void OnDisable()
    {
        PoolManager.Instance.PoolObject("evenDumberChaser", this);
    }
}
