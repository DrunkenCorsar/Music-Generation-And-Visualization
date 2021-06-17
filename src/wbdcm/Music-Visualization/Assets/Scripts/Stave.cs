using AudioHelm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public partial class MainScript : MonoBehaviour
{
    public partial class Stave
    {
        AudioHelm.HelmController helmController;

        List<GameObject> grids;
        List<GameObject> gridMarkers;

        List<Tile> tiles;
        List<GameObject> tileObjects;

        List<GameObject> highlights;
        List<Tile> highlightedTiles;
        List<GameObject> highlightLights;

        int loopDuration;
        int adjustedLoopDuration;
        float speed;
        int currentGridNumber;

        float timeExpired;

        public Stave(AudioHelm.HelmController helmController)
        {
            loopDuration = DEFAULT_LOOP_DURATION;
            Text durationLabel = GameObject.Find("Duration").GetComponent<Text>();
            durationLabel.text = "Duration: " + loopDuration + "s";

            speed = DEFAULT_SPEED;

            this.helmController = helmController;

            Inicialize();
        }

        /// <summary>
        /// Mathod that actually changes duration of theme. 
        /// Before this method called, duration is stored in the buffer only
        /// </summary>
        public void PinLoopDuration()
        {
            adjustedLoopDuration = loopDuration;
        }

        /// <summary>
        /// Method that reacts to Duration change button.
        /// Changes duration of current theme to next value (in buffer only)
        /// </summary>
        public void ChangeLoopDuration()
        {
            loopDuration *= 2;
            if (loopDuration > MAX_LOOP_DURATION)
                loopDuration = MIN_LOOP_DURATION;
            Text durationLabel = GameObject.Find("Duration").GetComponent<Text>();
            durationLabel.text = "Duration: " + loopDuration + "s";
        }

        /// <summary>
        /// Method that changes playback speed to new value
        /// </summary>
        /// <param name="newSpeed">New value of playback speed</param>
        public void ChangeSpeed(float newSpeed)
        {
            speed = newSpeed;
        }

        /// <summary>
        /// Resets whole stave to initial state. Re-generates melody
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < GRIDS_AT_SCENE_COUNT; i++)
            {
                Destroy(grids[0]);
                grids.RemoveAt(0);
                Destroy(gridMarkers[0]);
                gridMarkers.RemoveAt(0);
            }

            for (int i = highlightedTiles.Count - 1; i >= 0; i--)
            {
                highlightedTiles.RemoveAt(i);
                Destroy(highlightLights[i]);
                Destroy(highlights[i]);
                highlightLights.RemoveAt(i);
                highlights.RemoveAt(i);
            }
            
            for (int i = tileObjects.Count - 1; i >= 0; i--)
            {
                // Destroy(tileObjects[i]);
                tileObjects.RemoveAt(i);
            }

            ObjectPooler.Instance.ResetPools();

            Inicialize();
        }

        /// <summary>
        /// Moving current stave forward for during some time interval
        /// </summary>
        /// <param name="deltaTime">Given time interval</param>
        public void Move(float deltaTime)
        {
            deltaTime *= speed;

            timeExpired += deltaTime;
            if (timeExpired >= adjustedLoopDuration)
                timeExpired -= adjustedLoopDuration;

            for (int i = 0; i < tileObjects.Count; i++)
            {
                tileObjects[i].transform.position -= new Vector3(0f, GRID_UNIT_HEIGHT * deltaTime, 0f);
            }

            for (int i = 0; i < grids.Count; i++)
            {
                grids[i].transform.position -= new Vector3(0f, GRID_UNIT_HEIGHT * deltaTime, 0f);
                gridMarkers[i].GetComponent<Text>().rectTransform.anchoredPosition -= new Vector2(0f, GRID_MARKERS_DELTA_Y * deltaTime);

                if (grids[i].transform.position.y < GRID_DISPOSE_LEVEL)
                {
                    DisposeGridUnit(i);
                    AddNewGridUnit();
                }
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].IsPlayingAt(timeExpired))
                {
                    bool found = false;
                    for (int j = 0; j < highlightedTiles.Count; j++)
                        if (tiles[i].NotePitchAbsolute == highlightedTiles[j].NotePitchAbsolute)
                            found = true;
                    if (!found)
                    {
                        highlights.Add(Instantiate(GameObject.Find(tiles[i].HighlightTemplateName)));
                        highlights[highlights.Count - 1].transform.position = new Vector3(
                            tiles[i].XPosition, HIGHLIGHTS_POSITION_Y, 0f);
                        highlightLights.Add(Instantiate(GameObject.Find("HighlightLightTemplate")));
                        highlightLights[highlightLights.Count - 1].transform.position = new Vector3(
                            tiles[i].XPosition, HIGHLIGHT_LIGHT_Y, 0f);
                        highlightedTiles.Add(tiles[i]);

                        helmController.NoteOn(tiles[i].NotePitchAbsolute + HELM_DELTA,
                            0.7f + (UnityEngine.Random.value * 0.3f), tiles[i].FloatDuration);
                    }
                }
            }

            for (int i = 0; i < highlightedTiles.Count; i++)
            {
                if (!highlightedTiles[i].IsPlayingAt(timeExpired))
                {
                    Destroy(highlights[i]);
                    highlights.RemoveAt(i);
                    Destroy(highlightLights[i]);
                    highlightLights.RemoveAt(i);
                    highlightedTiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Cleans out whole grid unit
        /// </summary>
        /// <param name="index"></param>
        private void DisposeGridUnit(int index)
        {
            int deletedGridNumber = Convert.ToInt32(gridMarkers[index].GetComponent<Text>().text.ToString());
            DisposeTilesFromGrid(deletedGridNumber);

            Destroy(grids[index]);
            Destroy(gridMarkers[index]);
            grids.RemoveAt(index);
            gridMarkers.RemoveAt(index);
        }

        /// <summary>
        /// Adding empty grid to list
        /// </summary>
        private void AddNewGridUnit()
        {
            currentGridNumber++;
            if (currentGridNumber > adjustedLoopDuration)
                currentGridNumber = 1;

            GameObject original = GameObject.Find("GridTemplate");
            GameObject originalGridMarker = GameObject.Find("GridMarkerTemplate");
            GameObject canvas = GameObject.Find("CanvasForMarkers");

            // spawning GRIDS and GRID MARKERS
            grids.Add(Instantiate(original));
            gridMarkers.Add(Instantiate(originalGridMarker));

            int count = grids.Count;
            gridMarkers[count - 1].transform.SetParent(canvas.transform, false);
            gridMarkers[count - 1].GetComponent<Text>().text = currentGridNumber.ToString();
            gridMarkers[count - 1].GetComponent<Text>().rectTransform.anchoredPosition =
                gridMarkers[count - 2].GetComponent<Text>().rectTransform.anchoredPosition + new Vector2(0f, GRID_MARKERS_DELTA_Y);
            grids[count - 1].transform.position = grids[count - 2].transform.position + new Vector3(0f, GRID_UNIT_HEIGHT, 0f);

            RenderTilesInGrid(currentGridNumber);
        }

        /// <summary>
        /// Inicializing initial scene view, reseting all objects, geerates new melody
        /// </summary>
        private void Inicialize()
        {
            tiles = new List<Tile>();
            tileObjects = new List<GameObject>();
            GenerateTiles();

            highlights = new List<GameObject>();
            highlightedTiles = new List<Tile>();
            highlightLights = new List<GameObject>();

            timeExpired = 0f;

            grids = new List<GameObject>();
            gridMarkers = new List<GameObject>();
            currentGridNumber = GRIDS_AT_SCENE_COUNT;

            GameObject originalGrid = GameObject.Find("GridTemplate");
            GameObject originalGridMarker = GameObject.Find("GridMarkerTemplate");
            GameObject canvas = GameObject.Find("CanvasForMarkers");

            for (int i = 0; i < GRIDS_AT_SCENE_COUNT; i++)
            {
                // Spawning GRIDS and GRID MARKERS
                grids.Add(Instantiate(originalGrid));
                gridMarkers.Add(Instantiate(originalGridMarker));
                gridMarkers[i].transform.SetParent(canvas.transform, false);
                gridMarkers[i].GetComponent<Text>().text = (i + 1).ToString();

                if (i == 0)
                {
                    grids[i].transform.position = new Vector3(0f, GRID_START_Y, 0f);
                    gridMarkers[i].GetComponent<Text>().rectTransform.anchoredPosition = new Vector3(GRID_MARKER_START_X, GRID_MARKER_START_Y, 0f);
                }
                else
                {
                    grids[i].transform.position = grids[i - 1].transform.position + new Vector3(0f, GRID_UNIT_HEIGHT, 0f);
                    gridMarkers[i].GetComponent<Text>().rectTransform.anchoredPosition =
                        gridMarkers[i - 1].GetComponent<Text>().rectTransform.anchoredPosition + new Vector2(0f, GRID_MARKERS_DELTA_Y);
                }

                RenderTilesInGrid(i + 1);

                PinLoopDuration();
                // PinSpeeed();
            }
        }

        /// <summary>
        /// Renders all tiles in grid with given number
        /// </summary>
        /// <param name="gridNumber">Given grid index</param>
        private void RenderTilesInGrid(int gridNumber)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].BelongsToGrid(gridNumber))
                {
                    Tile currentTile = tiles[i];

                    GameObject newTileObject = ObjectPooler.Instance.SpawnFromPool(currentTile.TileObjectTemplateName,
                        new Vector3(currentTile.XPosition, currentTile.DeltaYPosition + grids[grids.Count - 1].transform.position.y, 0f));

                    newTileObject.name = "Tile" + i.ToString();

                    tileObjects.Add(newTileObject);
                }
            }
        }

        /// <summary>
        /// Cleans up grid with given number
        /// </summary>
        /// <param name="gridNumber">Given grid index</param>
        private void DisposeTilesFromGrid(int gridNumber)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].BelongsToGrid(gridNumber))
                {
                    int objIndex = -1;
                    for (int j = 0; j < tileObjects.Count; j++)
                    {
                        if (tileObjects[j].name == "Tile" + i)
                            objIndex = j;
                    }
                    if (objIndex != -1)
                    {
                        tileObjects.RemoveAt(objIndex);
                    }
                }
            }
        }
    }
}
