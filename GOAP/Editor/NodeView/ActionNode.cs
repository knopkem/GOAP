/**
 * Copyright 2017 Sword GC
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Author: Peter Klooster | CrashKonijn
 * Project: https://github.com/crashkonijn/GOAP
 */

using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SwordGC.AI.Core.Editor.NodeView
{
    public class ActionNode : Node
    {
        /// <summary>
        /// The corresponding action
        /// </summary>
        GoapAction action;

        override public string title
        {
            get { return action.Description; }
        }

        protected override GUIStyle GetStyle(GoapAction[] active)
        {
            if (action.isBlocked)
                return blockedStyle;
            else
                return active.Contains(action) ? activeStyle : style;
        }

        protected override string textColor
        {
            get
            {
                return action.proceduralConditionsValid ? "white" : "grey";
            }
        }

        public override string data
        {
            get
            {

                string effectsStr = "---- effs ----------\n";

                foreach (KeyValuePair<string, bool> entry in action.effects)
                {
                    string colorStr = "white";
                    string negateStr = (entry.Value ? "" : "!");
                    effectsStr += string.Format("    <color={0}>{1}{2}</color>\n", colorStr, negateStr, entry.Key);
                }

                //effectsStr += "------------------";


                string presStr = string.Format("---- pres: {0,5} ---\n", action.preconditionsValid);

                foreach (KeyValuePair<string, bool> entry in action.preconditions)
                {
                    string colorStr = (factSet.CheckFact(entry.Key, entry.Value) ? "green" : "red");
                    string negateStr = (entry.Value ? "" : "!");

                    presStr += string.Format("    <color={0}>{1}{2}</color>\n", colorStr, negateStr, entry.Key);
                }
                //presStr += "------------------";


                string s =  string.Format(
                    "\n" +
                    "trgt:  {2,12}\n" +
                    "cost:  {3,12:F2}\n" +
                    "range: {4,11:F2}\n" +
                    "\n" +
                    "{0}" +
                    "\n" +
                    "{1}",


                    effectsStr,
                    presStr,
                    action.targetName, 
                    action.cost,
                    action.requiredRange
                );

                return s;
            }
        }

        public ActionNode(Node parent, int depth, Node neighbour, GoapAction action, FactSet dataSet, GUIStyle nodeStyle, GUIStyle activeStyle, GUIStyle blockedStyle) : base(parent, depth, neighbour, dataSet, nodeStyle, activeStyle, blockedStyle)
        {
            this.action = action;
        }

        public override void Draw(GoapAction[] active)
        {
            if (action == null || action.agent == null) return;

            if (childs.Count == 0)
            {
                Handles.DrawLine(rect.center, rect.center + (Vector2.up * (rect.height / 1.2f)));
                GUI.Box(new Rect(rect.center + (Vector2.up * (rect.height / 1.5f) + new Vector2(-20, 0f)), new Vector2(40f, 25f)), "<color=white>" + Vector3.Distance(action.position, action.agent.transform.position).ToString("0.0") + "</color>", GetStyle(active));
            }

            base.Draw(active);

            if (parent != null)
            {
                Vector3 point = BezierPoint(0.5f,
                drawnRect.center + (Vector2.down * (drawnRect.height / 3f)),
                parent.drawnRect.center - (Vector2.down * (drawnRect.height / 3f)),
                drawnRect.center + (Vector2.down * 100f),
                parent.drawnRect.center - (Vector2.down * 100f));

                point.x -= 20f;

                GUI.Box(new Rect(point, new Vector2(40f, 40f)), "<color=white>" + action.Distance.ToString("0.0") + "</color>", GetStyle(active));
            }
        }

        /// <summary>
        /// Returns point t on a bezier curve
        /// </summary>
        private Vector3 BezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 point = Vector3.zero;

            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            point = uuu * p0;
            point += 3 * uu * t * p1;
            point += 3 * u * tt * p2;
            point += ttt * p3;

            return point;
        }
    }
}
