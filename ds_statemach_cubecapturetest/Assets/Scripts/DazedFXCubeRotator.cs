﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DazedFXCubeRotator : MonoBehaviour
{
void Update()
    {
        transform.Rotate(new Vector3(90,180,360) * Time.deltaTime);
    }
}
