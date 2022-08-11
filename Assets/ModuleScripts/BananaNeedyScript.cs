using System;
using System.Collections;
using System.Collections.Generic;
using KModkit;
using KeepCoding;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class BananaNeedyScript : ModuleScript
{
    private KMNeedyModule _Needy;
    private System.Random _Rnd;

    [SerializeField]
    private SpriteRenderer _ModuleSprite;
    [SerializeField]
    private KMSelectable _Button;

    private bool _isNeedyActive, _isSeedSet;
    private int _seed, _generatedPressCount;

    // Use this for initialization
    void Start()
    {
        if (!_isSeedSet)
        {
            _seed = Rnd.Range(int.MinValue, int.MaxValue);
            Log("The seed is: " + _seed.ToString());
            _isSeedSet = true;
        }

        _Rnd = new System.Random(_seed);
        // SET SEED ABOVE IN CASE OF BUGS!!
        // _rnd = new System.Random(loggedSeed);
        _Needy = Get<KMNeedyModule>();
        _Needy.OnNeedyActivation += OnNeedyActivation;
        _Needy.OnNeedyDeactivation += OnNeedyDeactivation;
        _Needy.OnTimerExpired += OnTimerExpired;
        _Button.Assign(onInteract: () => { ButtonPress(); });

        _ModuleSprite.color = new Color32(255, 255, 255, 255);
    }

    private void OnNeedyActivation()
    {
        _isNeedyActive = true;
        _generatedPressCount = Rnd.Range(15, 30);
        _ModuleSprite.color = new Color32(0, 255, 0, 255);
        Log("Need to press the banana " + _generatedPressCount.ToString() + " times.");
    }

    private void OnNeedyDeactivation()
    {
        _isNeedyActive = false;
        _generatedPressCount = 0;
        _ModuleSprite.color = new Color32(255, 255, 255, 255);
    }

    private void OnTimerExpired()
    {
        _isNeedyActive = false;
        _ModuleSprite.color = new Color32(255, 255, 255, 255);
        if(_generatedPressCount == 0)
        {
            Log("j");
            _Needy.HandlePass();
        }
        else
        {
            Log("Ran out of time. Strike.");
            _Needy.HandleStrike();
        }
    }

    private void ButtonPress()
    {
        ButtonEffect(_Button, 0.5f, KMSoundOverride.SoundEffect.ButtonPress);
        if(_isNeedyActive)
        {
            _generatedPressCount--;
            _ModuleSprite.color = new Color32((byte)((30 - _generatedPressCount) * 8), 255, 0, 255);
            if (_generatedPressCount == 0)
            {
                _ModuleSprite.color = new Color32(255, 0, 0, 255);
                Log("Pressed the banana enough times. Please stop now.");
            }
            else if(_generatedPressCount < 0)
            {
                Log("Pressed the banana when not needed. Strike,");
                _Needy.HandleStrike();
                _Needy.HandlePass();
            }
        }
        else
        {
            Log("I am a cruel person, so I will strike you, even though the module is inactive.");
            _Needy.HandleStrike();
        }
    }
}
