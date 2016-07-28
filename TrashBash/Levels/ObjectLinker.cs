using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;

namespace TrashBash.Levels
{
    class ObjectLinker
    {
        private Body body;
        private Geom geom;

        private Vector2 position;
        private float rotation;

        public ObjectLinker(Body body)
        {
            this.body = body;
            this.geom = null;
            Synchronize();
        }

        public ObjectLinker(Geom geom)
        {
            this.body = null;
            this.geom = geom;
            Synchronize();
        }

        public Vector2 Position
        {
            get { return this.position; }
        }

        public float Rotation
        {
            get { return this.rotation; }
        }

        public void Synchronize()
        {
            if (body != null)
            {
                this.position = this.body.Position;
                this.rotation = this.body.Rotation;
            }
            else
            {
                this.position = this.geom.Position;
                this.rotation = this.geom.Rotation;
            }
        }
    }
}
