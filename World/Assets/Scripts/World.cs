using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class World : MonoBehaviour {

    public GameObject cubeGrass;
    public GameObject cubeStone;
    public GameObject cubeWater;
    public GameObject cubeSand;
    public GameObject tree;

    public int height = 10;
    public int width = 10;
    private int bottom = -25;


    private System.Random rand;

	// Use this for initialization
	void Start () {
        PerlinNoise noise = new PerlinNoise(200);
        rand = new System.Random();
        
        double seed = rand.NextDouble();
        double seedForest = rand.NextDouble();
        
        // Make a square of stone at the bottom of the world
        fillBottomWithStone(width, height, bottom);
        
        // Loop on all points of the rectancle representing the world
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Values to be used with perlin noise
                double nx = (double)i / 100 - 0.5;
                double ny = (double)j / 100 - 0.5;

                // Two octaves of perlin noise for the elevation because it look better
                float elevationValue = (float)(noise.Noise(nx * 4, ny * 4, seed) + 0.5 * noise.Noise(nx * 8, ny * 8, seed));

                // Only one octave for the forest because it didn't seem to change much
                int forest = Mathf.FloorToInt((float)noise.Noise(nx * 4, ny * 4, seedForest) * 10);

                // Apply a power to the elevation to make plains
                int elevation = Mathf.FloorToInt(Mathf.Pow(elevationValue * 8, 3f));


                // For each elevation a different block is assigned
                // TODO: Redo it since it's dirty
                GameObject cube;

                if (elevation < -2) // Water
                {
                    // Water always have the same elevation
                    elevation = -2;
                    cube = Instantiate(cubeWater) as GameObject;
                    fillBorder(i, j, elevation - 1);

                }
                else if (elevation < -1) // Sand
                {
                    cube = Instantiate(cubeSand) as GameObject;
                    fillBorder(i, j, elevation - 1);
                }
                else if (elevation < 7) // Grass
                {
                    cube = Instantiate(cubeGrass) as GameObject;

                    fillBorder(i, j, elevation - 1);
                    fillHole(i, j, elevation, 6, cubeGrass);

                    createTree(i, j, elevation, forest);
                }
                else // Stone
                {
                    cube = Instantiate(cubeStone) as GameObject;

                    fillBorder(i, j, elevation - 1);
                    fillHole(i, j, elevation, 12, cubeStone);
                }

                cube.transform.position = new Vector3(i, elevation, j);
            }
        }
    }

    // Try to create a tree based on the forestSeed
    private void createTree(int i, int j, int elevation, float forest)
    {
        int chance = rand.Next(1000);

        if (forest < 0) // When it is not a forest
        {
            if (chance < 5)
            {
                GameObject newTree = Instantiate(tree) as GameObject;
                newTree.transform.position = new Vector3(i, elevation, j);
            }
        }
        else // When it is a forest
        {
            if (chance < 50)
            {
                GameObject newTree = Instantiate(tree) as GameObject;
                newTree.transform.position = new Vector3(i, elevation, j);
            }
        }
    }

    // Fill Holes that may arise from too much differnce in elevation
    private void fillHole(int x, int y, int point, int amplitude, GameObject cube)
    {
        if (point > bottom + amplitude)
        {
            fillDownWithCube(x, y, point - 1, point - amplitude, cube);
        }
    }

    // Fill the border of the map with stones
    private void fillBorder(int x, int y, int start)
    {
        if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
        {
            fillDownWithCube(x, y, start, bottom, cubeStone);
        }
    }
	
    // Fill verticaly with cube
    private void fillDownWithCube(int x, int y, int start, int end, GameObject cube)
    {
        for (; start >= end; start--)
        {
            GameObject newCube = Instantiate(cube) as GameObject;
            newCube.transform.position = new Vector3(x, start, y);
        }
    }

    // Make rectangle of stone at the bottom of the map
    private void fillBottomWithStone(int x, int y, int z)
    {
        for (int  i = 1; i < x - 1; i++)
        {
            for (int j = 1 ; j < y - 1; j++)
            {
                GameObject cube = Instantiate(cubeStone) as GameObject;
                cube.transform.position = new Vector3(i, z, j);
            }
        }
    }
}
