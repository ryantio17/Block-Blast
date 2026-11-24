using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratulationsScript : MonoBehaviour
{

    public List<GameObject>writtings;

    void Start()
    {
        TheGameEvents.ShowCongratulationsWritings += ShowCongratulationsWritings;
    }

    private void OnDisable()
    {
        TheGameEvents.ShowCongratulationsWritings -= ShowCongratulationsWritings;
    }

    private void ShowCongratulationsWritings()
    {
        var index = UnityEngine.Random.Range(0,writtings.Count);
        writtings[index].SetActive(true);
    }
}
