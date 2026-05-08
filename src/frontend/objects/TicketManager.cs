using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fleasociety
{
    public class Ticket
    {
        public byte customerId;
        public Ticket()
        {
        }
    }
    public class TicketObject
    {
        // Constants
        private static readonly Vector2 initialPosition = new Vector2(43, 4);
        private static Vector2 ticketSize = new Vector2(0, 0);
        private static Rectangle placementBounds = new Rectangle(43, 4, 189, 53);
        private static float animatingSpeed = 0.5f;
        private static bool[] occupiedSlots = new bool[256];
        private static int precision = 16;

        // Variables
        public Ticket ticket;
        private Vector2 position = new Vector2(0, 0);
        private Vector2 grabOffset = new Vector2(0, 0);
        private bool grabbing = false;
        private bool animating = false;

        // Graphics
        private static Texture2D? pixel = null;
        private static Texture2D? gfxTicket = null;
        private static Texture2D? gfxTicket1 = null;
        private static Texture2D? gfxTicket2 = null;
        private static Color ticketColor = Color.White;
        private static Color ticketShadowColor = Color.Black;

        public TicketObject(Ticket ticket)
        {
            this.ticket = ticket;
            position = initialPosition;
            animating = true;
        }
        public bool Update(GameTime gameTime, bool handleInput)
        {
            // Animate the ticket moving from its current position to the closest valid position within the placement bounds.
            if(animating)
            {
                int targetX = (int)MathHelper.Clamp(position.X, placementBounds.Left, placementBounds.Right - ticketSize.X);
                // Decrease prescision so that tickets only have so many "slots" they can be placed in, which makes it easier to place them without needing pixel-perfect precision.
                targetX = targetX / precision * precision;
                // If the target slot is occupied, find the nearest unoccupied slot.
                bool attemptsFailed = false;
                bool left = targetX < placementBounds.Left + placementBounds.Width / 2;
                while (occupiedSlots[targetX])
                {
                    GlobalContent.PlaySound("Error");
                    // if it's in the left half, move right, otherwise move left.
                    if (left)
                    {
                        targetX += precision;
                        if (targetX > placementBounds.X + placementBounds.Width)
                        {
                            targetX = placementBounds.Left;
                            if(attemptsFailed)
                                break;
                            attemptsFailed = true;
                        }
                    }
                    else
                    {
                        targetX -= precision;
                        if (targetX < placementBounds.X)
                        {
                            targetX = placementBounds.Right;
                            if(attemptsFailed)
                                break;
                            attemptsFailed = true;
                        }
                    }
                }
                Vector2 targetPosition = new Vector2(
                    targetX,
                    MathHelper.Clamp(position.Y, placementBounds.Top, placementBounds.Bottom - ticketSize.Y)
                );
                position = Vector2.Lerp(position, targetPosition, animatingSpeed);
                if(!occupiedSlots[targetX] && Vector2.Distance(position, targetPosition) < 0.5f)
                {
                    position = targetPosition;
                    animating = false;
                    occupiedSlots[(int)position.X] = true;
                }
                return false;
            }
            if(!handleInput)
            {
                return false;
            }
            Vector2 rescaledMousePos = Input.MouseState.Position.ToVector2() / GlobalGraphics.scale;
            // If grabbing, move the ticket with the mouse.
            if (grabbing)
            {
                position = rescaledMousePos - grabOffset;

                // Stop grabbing if the mouse button is released.
                if (Input.LastMouseState.LeftButton == ButtonState.Pressed && Input.MouseState.LeftButton == ButtonState.Released)
                {
                    grabbing = false;
                    animating = true;
                    GlobalContent.PlaySound("CompatSelect");
                }
            }
            else if (GlobalGraphics.Scale(new Rectangle((int)position.X, (int)position.Y, (int)ticketSize.X, (int)ticketSize.Y)).Contains(Input.MouseState.Position))
            {
                // Start grabbing if the mouse button is pressed.
                if (Input.LastMouseState.LeftButton == ButtonState.Released && Input.MouseState.LeftButton == ButtonState.Pressed)
                {
                    occupiedSlots[(int)position.X] = false;
                    grabbing = true;
                    animating = false;
                    grabOffset = rescaledMousePos - position;
                    GlobalContent.PlaySound("Option");
                }
            }
            return grabbing;
        }
        public void DrawLayer1(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gfxTicket2 == null)
                return;
            spriteBatch.Draw(pixel, GlobalGraphics.Scale(new Rectangle((int)position.X, (int)position.Y, (int)(ticketSize.X + 1), (int)(ticketSize.Y + 1))), ticketShadowColor);
            spriteBatch.Draw(gfxTicket2, GlobalGraphics.Scale(new Rectangle((int)position.X + gfxTicket2.Bounds.Size.X, (int)position.Y, gfxTicket2.Bounds.Width, gfxTicket2.Bounds.Height)), ticketColor);
        }
        public void DrawLayer2(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gfxTicket == null
                || gfxTicket1 == null)
                return;
            spriteBatch.Draw(gfxTicket1, GlobalGraphics.Scale(new Rectangle((int)position.X, (int)position.Y, gfxTicket1.Bounds.Width, gfxTicket1.Bounds.Height)), ticketColor);
            spriteBatch.Draw(gfxTicket, GlobalGraphics.Scale(new Rectangle((int)position.X, (int)position.Y + gfxTicket1.Bounds.Height, gfxTicket.Bounds.Width, gfxTicket.Bounds.Height)), ticketColor);

            // Draw customer id as #0 - #255
            string customerIdText = $"#{ticket.customerId+1}";
            GlobalGraphics.DrawString(spriteBatch, L.FontSmall(), customerIdText, GlobalGraphics.Scale(new Vector2(position.X + 1, position.Y + 7)), Color.Red);

            // If the mouse is hovering over the ticket, show a tooltip.
            if (!grabbing && GlobalGraphics.Scale(new Rectangle((int)position.X, (int)position.Y, (int)ticketSize.X, (int)ticketSize.Y)).Contains(Input.MouseState.Position))
            {
                Global.tooltip = "Grab ticket";
            }
        }
        public static void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            pixel = GlobalContent.GetTexture("Pixel");
            gfxTicket = GlobalContent.AddTexture("Ticket", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticket"));
            gfxTicket1 = GlobalContent.AddTexture("Ticket1", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticket1"));
            gfxTicket2 = GlobalContent.AddTexture("Ticket2", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticket2"));
            ticketColor = ThemeManager.GetColor("OrderStationTicket");
            ticketShadowColor = ThemeManager.GetColor("OrderStationTicketShadow");
            ticketSize = new Vector2(gfxTicket.Bounds.Width, gfxTicket.Bounds.Height + gfxTicket1.Bounds.Height);
        }
        public Vector2 GetPosition()
        {
            return position;
        }
    }
    public static class TicketManager
    {
        private static List<TicketObject> tickets = new List<TicketObject>();
        private static Texture2D? gfxTicketFull = null;
        private static Texture2D? gfxTicketHolders = null;
        private static Texture2D? gfxTicketHolders2 = null;
        public static bool Update(GameTime gameTime, bool handleInput)
        {
            for (int i = tickets.Count - 1; i >= 0; i--)
            {
                if(tickets[i].Update(gameTime, handleInput))
                {
                    break;
                }
            }
            // Re-order the tickets so that the leftmost ticket is at the back and the rightmost ticket is at the front.
            tickets.Sort((a, b) => a.GetPosition().X.CompareTo(b.GetPosition().X));
            return false;
        }
        public static TicketObject Add(Ticket ticket)
        {
            ticket.customerId = Next();
            TicketObject ticketObject = new(ticket);
            tickets.Add(ticketObject);
            return ticketObject;
        }
        public static void Clear()
        {
            tickets.Clear();
        }
        public static byte Next()
        {
            return (byte)(tickets.Count % 256);
        }
        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gfxTicketFull == null
                || gfxTicketHolders == null
                || gfxTicketHolders2 == null)
                return;

            for (int i = 0; i < tickets.Count; i++)
            {
                tickets[i].DrawLayer1(gameTime, spriteBatch);
            }
            spriteBatch.Draw(gfxTicketHolders, GlobalGraphics.Scale(new Rectangle(32, 0, gfxTicketHolders.Bounds.Width, gfxTicketHolders.Bounds.Height)), ThemeManager.GetColor("OrderStationTicketHolders"));
            for (int i = 0; i < tickets.Count; i++)
            {
                tickets[i].DrawLayer2(gameTime, spriteBatch);
            }
            //spriteBatch.Draw(gfxTicketFull, GlobalGraphics.Scale(new Rectangle(240, 4, gfxTicketFull.Bounds.Width, gfxTicketFull.Bounds.Height)), ThemeManager.GetColor("OrderStationTicket"));

            spriteBatch.Draw(gfxTicketHolders2, GlobalGraphics.Scale(new Rectangle(32, 0, gfxTicketHolders2.Bounds.Width, gfxTicketHolders2.Bounds.Height)), ThemeManager.GetColor("OrderStationTicketHolders2"));
        }
        public static void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            TicketObject.LoadContent(contentManager, graphicsDevice);
            gfxTicketFull = GlobalContent.AddTexture("TicketFull", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticketfull"));
            gfxTicketHolders = GlobalContent.AddTexture("TicketHolders", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticketholders"));
            gfxTicketHolders2 = GlobalContent.AddTexture("TicketHolders2", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticketholders2"));
        }
    }
}
