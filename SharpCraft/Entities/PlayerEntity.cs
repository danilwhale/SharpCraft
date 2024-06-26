﻿using System.Numerics;

namespace SharpCraft.Entities;

public class PlayerEntity : Entity
{
    private const float MouseSensitivity = 0.075f;
    private const float HalfWidth = 0.3f;
    private const float HalfHeight = 0.9f;
    
    public Camera3D Camera;

    public PlayerEntity(Level.Level level) : base(level, HalfWidth, HalfHeight)
    {
        Camera = new Camera3D(Vector3.Zero, Vector3.Zero, Vector3.UnitY, 70.0f, CameraProjection.Perspective);
        EyeLevel = 1.62f;
        MoveToRandom();
    }

    private void MoveToRandom()
    {
        var x = Random.Shared.NextSingle() * Level.Width;
        var y = Level.Height + 10;
        var z = Random.Shared.NextSingle() * Level.Length;
        MoveTo(new Vector3(x, y, z));
    }
    
    public void MoveCamera(float lastDelta)
    {
        Camera.Position = LastPosition + (Position - LastPosition) * lastDelta;

        var rotation = Matrix4x4.CreateFromYawPitchRoll(
            Yaw * DEG2RAD,
            Pitch * DEG2RAD,
            0.0f
        );

        var forward = Vector3.Transform(Vector3.UnitZ, rotation);

        Camera.Target = Camera.Position + forward;
    }

    public override void Rotate(float pitch, float yaw)
    {
        base.Rotate(pitch * MouseSensitivity, yaw * MouseSensitivity);
        Pitch = Math.Clamp(Pitch, -89.0f, 89.0f);
    }

    public override void Tick()
    {
        base.Tick();
        
        var x = IsKeyDown(KeyboardKey.A) || IsKeyDown(KeyboardKey.Left) ? 1
            : IsKeyDown(KeyboardKey.D) || IsKeyDown(KeyboardKey.Right) ? -1
            : 0;

        var z = IsKeyDown(KeyboardKey.W) || IsKeyDown(KeyboardKey.Up) ? 1
            : IsKeyDown(KeyboardKey.S) || IsKeyDown(KeyboardKey.Down) ? -1
            : 0;

        if (IsOnGround && (IsKeyDown(KeyboardKey.Space) || IsKeyDown(KeyboardKey.LeftSuper)))
        { 
            Direction.Y = 0.16f;
        }

        MoveRelative(x, z, IsOnGround ? 0.023f : 0.007f);
        Direction.Y -= 0.008f;
        Move(Direction);
        
        Direction *= new Vector3(0.91f, 0.98f, 0.91f);

        if (IsOnGround) Direction *= new Vector3(0.8f, 1.0f, 0.8f);
    }

    public void Update()
    {
        if (IsKeyPressed(KeyboardKey.R)) MoveToRandom();
    }
}