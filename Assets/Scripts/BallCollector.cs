using UnityEngine;

public class BallCollector : MonoBehaviour
{
    public bool hasBall = false;

    public void ReceiveBall()
    {
        hasBall = true;
        Debug.Log(this.gameObject.name + " has collected the ball!");
    }
}