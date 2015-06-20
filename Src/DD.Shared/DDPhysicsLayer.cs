//
//  DDPhysicsLayer.cs
//
//  DD engine for 2d games and apps: https://code.google.com/p/dd-engine/
//
//  Copyright (c) 2013 - Alexander Kirienko
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
#if DD_LIBRARY_BOX2D_XNA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using b2World = Box2D.XNA.World;
using b2Body = Box2D.XNA.Body;
using b2BodyDef = Box2D.XNA.BodyDef;
using b2Contact = Box2D.XNA.Contact;
using b2ContactImpulse = Box2D.XNA.ContactImpulse;
using b2Manifold = Box2D.XNA.Manifold;
using b2IContactListener = Box2D.XNA.IContactListener;
using b2Vec2 = Microsoft.Xna.Framework.Vector2;

class DDPhysicsNode : DDNode
{
    public DDPhysicsLayer World { get; private set; }
    public b2Body Body { get; private set; }

    public DDPhysicsNode(DDPhysicsLayer world, b2Body body)
    {
        this.World = world;
        this.Body = body;
        body.SetUserData(this);
    }

    public void ApplyLinearVelocity(float x, float y)
    {
        this.Body.SetLinearVelocity(new b2Vec2(x / World.WorldScale, y / World.WorldScale));
    }

    public void ApplyPosition(float x, float y)
    {
        this.Body.Position = new b2Vec2(x / World.WorldScale, y / World.WorldScale);
    }

    public void ApplyPosition(DDVector v)
    {
        ApplyPosition(v.x, v.y);
    }
    public event Action<DDPhysicsNode, DDVector> OnContact;

    internal void BeginContact(DDPhysicsNode other, DDVector pos)
    {
        if (OnContact != null)
            OnContact(other, pos);
    }

    internal void DestroyBody()
    {
        if (this.Body != null)
            World.DestroyBody(this.Body);
        this.Body = null;
        this.RemoveFromParent();
    }

    internal void UpdateFromBody()
    {
        if (Body != null)
        {
            Position = Body.Position * World.WorldScale;
            Rotation = Body.Rotation;
        }
    }
}

class DDPhysicsLayer : DDNode, b2IContactListener
{
    public b2World World { get; private set; }
    public float WorldScale { get; private set; }

    private List<b2Body> _bodiesToDestroy = new List<b2Body>();

    public DDPhysicsLayer(float worldScale = 128)
    {
        WorldScale = worldScale;
        World = new b2World(new b2Vec2(0, 0), false);
        World.ContactListener = this;
        this.RunAction(new DDUpdate(() => { UpdateWorld(); }));
    }

    private void UpdateWorld()
    {
        foreach (var physNode in _bodiesToDestroy)
        {
            World.DestroyBody(physNode);            
        }
        _bodiesToDestroy.Clear();
        World.Step(1f / DDDirector.Instance.FrameRate, 4, 4);

    }

    private DDPhysicsNode WrapBody(b2Body body)
    {
        var node = new DDPhysicsNode(this, body);
        AddChild(node);
        node.Position = body.Position * WorldScale;

        node.RunAction(new DDUpdate(() =>
        {
            node.UpdateFromBody();
        }));
        return node;
    }

    public DDPhysicsNode AddStaticBox(float x1, float y1, float x2, float y2)
    {
        var bodyDef = new b2BodyDef()
        {
            type = Box2D.XNA.BodyType.Static,
            position = new b2Vec2((x1 + x2) / 2 / WorldScale, (y1 + y2) / 2 / WorldScale),
            angle = 0,
            linearVelocity = new b2Vec2(0, 0),
            bullet = false,
        };

        var body = World.CreateBody(bodyDef);
        var shape = new Box2D.XNA.PolygonShape();
        shape.SetAsBox(Math.Abs(x2 - x1) / 2 / WorldScale, Math.Abs(y2 - y1) / 2 / WorldScale);

        var fixtureDef = new Box2D.XNA.FixtureDef()
        {
            shape = shape,
            density = 1,
            friction = 1,
            restitution = 0,
        };
        var fixture = body.CreateFixture(fixtureDef);

        return WrapBody(body);
    }

    public DDPhysicsNode AddBox(Box2D.XNA.BodyType t, DDVector pos, DDVector size)
    {
        var wpos = pos / WorldScale;
        var wsize = size / WorldScale;

        var bodyDef = new b2BodyDef()
        {
            type = t,
            position = wpos,
            angle = 0,
            linearVelocity = new b2Vec2(0, 0),
            bullet = false,
        };

        var body = World.CreateBody(bodyDef);
        var shape = new Box2D.XNA.PolygonShape();
        
        shape.SetAsBox(wsize.Width, wsize.Height);

        var fixtureDef = new Box2D.XNA.FixtureDef()
        {
            shape = shape,
            density = 1,
            friction = 1,
            restitution = 0,
        };
        var fixture = body.CreateFixture(fixtureDef);

        return WrapBody(body);
    }

    public DDPhysicsNode AddStaticCircle(float x, float y, float r)
    {
        var bodyDef = new b2BodyDef()
        {
            type = Box2D.XNA.BodyType.Static,
            position = new b2Vec2(x / WorldScale, y / WorldScale),
            angle = 0,
            linearVelocity = new b2Vec2(0, 0),
            bullet = false,
        };

        var body = World.CreateBody(bodyDef);
        var shape = new Box2D.XNA.CircleShape();
        shape._radius = r / WorldScale;

        var fixtureDef = new Box2D.XNA.FixtureDef()
        {
            shape = shape,
            density = 1,
            friction = 1,
            restitution = 0,
        };
        var fixture = body.CreateFixture(fixtureDef);

        return WrapBody(body);
    }

    public DDPhysicsNode AddDynamicCircle(float x, float y, float r, bool isBullet = false)
    {
        var bodyDef = new b2BodyDef()
        {
            type = Box2D.XNA.BodyType.Dynamic,
            position = new b2Vec2(x / WorldScale, y / WorldScale),
            angle = 0,
            linearVelocity = new b2Vec2(0, 0),
            bullet = isBullet,
        };

        var body = World.CreateBody(bodyDef);
        var shape = new Box2D.XNA.CircleShape();
        shape._radius = r / WorldScale;

        var fixtureDef = new Box2D.XNA.FixtureDef()
        {
            shape = shape,
            density = 1,
            friction = 1,
            restitution = 0,
        };
        var fixture = body.CreateFixture(fixtureDef);

        return WrapBody(body);
    }

    public void BeginContact(b2Contact contact)
    {
        var node1 = contact.GetFixtureA().GetBody().GetUserData() as DDPhysicsNode;
        var node2 = contact.GetFixtureB().GetBody().GetUserData() as DDPhysicsNode;

        node1.UpdateFromBody();
        node2.UpdateFromBody();

        var pos = DDVector.Zero;
        //Manifold manifold;
        //contact.GetManifold(out manifold);
        //var pos = new DDVector(manifold._localPoint) * WorldScale;

        ////contact.GetFixtureA()

        //var fixture1 = contact.GetFixtureA();
        //var fixture2 = contact.GetFixtureB();

        //if (fixture1.ShapeType == ShapeType.Circle && fixture2.ShapeType == ShapeType.Circle)
        //{
        //    var r1 = fixture1._shape._radius;
        //    var r2 = fixture1._shape._radius;
        //    pos = node1.Position + (node2.Position - node1.Position) * (r1 / (r2 + r1));
        //}

        if (node1 != null)
            node1.BeginContact(node2, pos);

        if (node2 != null)
            node2.BeginContact(node1, pos);

        //throw new NotImplementedException();
    }

    public void EndContact(b2Contact contact)
    {
        //throw new NotImplementedException();
    }

    public void PreSolve(b2Contact contact, ref b2Manifold oldManifold)
    {
        //throw new NotImplementedException();
    }

    public void PostSolve(b2Contact contact, ref b2ContactImpulse impulse)
    {
        //throw new NotImplementedException();
    }

    internal void DestroyBody(b2Body body)
    {
        _bodiesToDestroy.Add(body);
    }
}

#endif