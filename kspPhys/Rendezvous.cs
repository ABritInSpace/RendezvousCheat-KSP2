using System;
using System.Linq;
using KSP;
using KSP.Game;
using KSP.Map.impl;
using KSP.Rendering;
using KSP.Sim.impl;
using UnityEngine;
using SpaceWarp.API;
using System.Runtime.CompilerServices;
using KSP.Logging;
using KSP.Sim;
using SpaceWarp.API.Mods;

//WHERE INDICATED BY **1**, RELEVANT SECTIONS HAVE BEEN MODIFIED FROM LazyOrbit.
//SEE README FOR DETAILS

namespace kspPhys
{
    [MainMod]
    public class Rendezvous : Mod
    {
        public bool lod = false;
        public Rect window;
        public string displayDist;
        public float dist;
        public bool selecting = false;
        public VesselComponent target;
        public Vector2 scrollpos;
        public bool doDraw = false;
        public override void Initialize()
        {
            //**1**
            if (lod)
                Destroy(this);
            lod = true;
            //
        }
        void Awake()
        {
            //**1**
            window = new Rect((Screen.width * 0.85f) - (75), (Screen.height / 2) - (300), 0, 0);
            //
        }
        void Update()
        {
            //**1**
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.R))
            {
                doDraw = !doDraw;
            }
            //
        }

        void OnGUI()
        {
            //**1**
            if (doDraw)
            {
                window = GUILayout.Window(GUIUtility.GetControlID(FocusType.Passive),
                    window,
                    draw,
                    "Rendezvous Cheat",
                    GUILayout.Height(0),
                    GUILayout.Width(150));
            }
            //
        }

        void draw(int id)
        {
            GameInstance game = GameManager.Instance.Game;

            GUILayout.BeginVertical();
            
            //**1**
            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance, m: ", GUILayout.Width(75));
            //
            displayDist = GUILayout.TextField(displayDist);
            //**1**
            float.TryParse(displayDist, out dist);
            //
            GUILayout.EndHorizontal();
            GUILayout.Label("Select a vessel to rendezvous with:");

            GUILayout.EndVertical();

            VesselComponent[] vessels = game.SpaceSimulation.UniverseModel.GetAllVessels().ToArray();
            
            //**1**
            GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
            scrollpos = GUILayout.BeginScrollView(scrollpos, false, true, GUILayout.Height(100), GUILayout.Width(150));
            //
            foreach (VesselComponent v in vessels)
            {
                if (v.Guid.ToString() != game.ViewController.GetActiveVehicle(true)?.Guid.ToString())
                {
                    if (GUILayout.Button(v.Name))
                    {
                        target = v;
                        DoRendz();
                    }
                }
                else
                {
                    GUIStyle invalid = new GUIStyle(GUI.skin.button);
                    invalid.normal.textColor = Color.red;
                    GUILayout.Button(v.Name, invalid);
                }
            }
            //**1**
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            //
        }

        void DoRendz()
        {
            GameInstance game = GameManager.Instance.Game;

            game.SpaceSimulation.Lua.TeleportToRendezvous(game.ViewController.GetActiveVehicle(true)?.Guid.ToString(),
                target?.Guid.ToString(),
                dist,
                0,
                0,
                0,
                0,
                0);
        }
    }
}
