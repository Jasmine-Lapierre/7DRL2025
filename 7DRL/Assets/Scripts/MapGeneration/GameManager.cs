using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<MapNode> MapNodes { get; private set; }
    public List<(MapNode, MapNode)> MapLinks { get; private set; }
    public PlayerInfo Player { get; private set; }
    public GameObject playerPrefab;
    private PlayerController playerInstance;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameManager();
        }
    }

    public void SpawnPlayer(MapNode startNode)
    {
        if (startNode == null)
        {
            Debug.LogError("Cannot spawn player: start node is null!");
            return;
        }

        if (playerInstance == null && playerPrefab != null)
        {
            // Instantiate exactly at the node's transform position
            Vector3 spawnPosition = startNode.transform.position;
            GameObject playerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            playerInstance = playerObj.GetComponent<PlayerController>();
            
            if (playerInstance == null)
            {
                return;
            }

            // Make sure the player stays at the exact node position
            playerInstance.transform.position = spawnPosition;
            playerInstance.InitializePosition(startNode);
            SetPlayerPosition(startNode);
        }
    }

    public void InitializeGameManager()
    {
        MapNodes = new List<MapNode>();
        MapLinks = new List<(MapNode, MapNode)>();
        Player = new PlayerInfo();
        playerInstance = null;  // Reset player instance reference
    }

    public void AddMapNode(MapNode node)
    {
        MapNodes.Add(node);
    }

    public void AddMapLink(MapNode node1, MapNode node2)
    {
        MapLinks.Add((node1, node2));
    }

    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        Player = playerInfo;
    }

    public void SetPlayerPosition(MapNode node)
    {
        Player.CurrentNode = node;
    }

    public MapNode GetPlayerPosition()
    {
        return Player.CurrentNode;
    }

    internal void UpdateMapNode(MapNode mapNode)
    {
        //retrouver le node dans la liste
        var node = MapNodes.Find(n => n == mapNode);
        if (node != null)
        {
            node = mapNode;
        }
    }

    public void DrawMap(Vector3 offset)
    {
        foreach (var link in MapLinks)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(link.Item1.position + offset, link.Item2.position + offset);
        }

        foreach (var node in MapNodes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node.position + offset, 0.2f);
        }
    }
}

public class PlayerInfo
{
    public int Score { get; set; }
    public int Level { get; set; }
    public MapNode CurrentNode { get; set; } // Position actuelle du joueur
}
