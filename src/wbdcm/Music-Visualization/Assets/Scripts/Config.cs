using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public partial class MainScript : MonoBehaviour
{
    private const int HELM_DELTA = 33;

    private const int DEFAULT_LOOP_DURATION = 64;
    private const int MAX_LOOP_DURATION = 256;
    private const int MIN_LOOP_DURATION = 8;

    private const float DEFAULT_SPEED = 0.75f;
    private const float MIN_SPEED = 0.25f;
    private const float MAX_SPEED = 2.0f;

    private const int TOTAL_KEYS_AT_KEYBOARD = 80;

    private const int GRIDS_AT_SCENE_COUNT = 5;

    private const float HIGHLIGHTS_POSITION_Y = -3.295556f;
    private const float GRID_UNIT_HEIGHT = 3.22f;
    private const float GRID_DISPOSE_LEVEL = -7.5f;
    private const float GRID_START_Y = -0.6155556f;
    private const float GRID_MARKERS_DELTA_Y = 199f;
    private const float GRID_MARKER_START_Y = -134f;
    private const float GRID_MARKER_START_X = -569f;

    private const float HIGHLIGHT_LIGHT_Y = -2.24f;
}