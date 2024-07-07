﻿using System.Numerics;
using SharpCraft.Rendering;
using SharpCraft.Tiles;
using SharpCraft.Utilities;

namespace SharpCraft.Level;

public sealed class Chunk : IDisposable
{
    public const int Size = 16;

    public static int Updates;
    public static int Rebuilds;

    public readonly int X;
    public readonly int Y;
    public readonly int Z;
    
    public readonly int MaxX;
    public readonly int MaxY;
    public readonly int MaxZ;

    public readonly BoundingBox BBox;
    public bool IsDirty = true;

    private readonly Level _level;
    private readonly MeshBuilder _builder = new();

    public Chunk(Level level, int x, int y, int z)
    {
        _level = level;
        X = x << 4;
        Y = y << 4;
        Z = z << 4;
        MaxX = (x + 1) << 4;
        MaxY = (y + 1) << 4;
        MaxZ = (z + 1) << 4;
        BBox = new BoundingBox(new Vector3(X, Y, Z), new Vector3(MaxX, MaxY, MaxZ));
    }

    private void Rebuild()
    {
        if (Rebuilds >= 2) return;
        
        Updates++;
        Rebuilds++;
        
        _builder.Begin();
        
        for (var x = X; x < MaxX; x++)
        {
            for (var y = Y; y < MaxY; y++)
            {
                for (var z = Z; z < MaxZ; z++)
                {
                    var tile = _level.GetTile(x, y, z);
                    TileRegistry.Registry[tile]?.Build(_builder, _level, x, y, z);
                }
            }
        }
        
        _builder.End();
        
        IsDirty = false;
    }

    public void Draw()
    {
        if (IsDirty)
        {
            Rebuild();
        }
        
        _builder.Draw(Assets.GetTextureMaterial("terrain.png"));
    }

    public void Dispose()
    {
        _builder.Dispose();
    }
}