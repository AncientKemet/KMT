﻿using System;
using Client.Net;
using Libaries.Net.Packets.ForServer;

namespace Client.UI.Controls.Items
{
    public static class ItemDragManager
    {
        private static ItemButton _indragButton;

        public static ItemButton IndragButton
        {
            get { return _indragButton; }
            private set { _indragButton = value; }
        }

        /// <summary>
        /// Called by ItemButton when the drag starts.
        /// </summary>
        public static void DragBegin(ItemButton button)
        {
            if (IndragButton != null)
                throw new Exception("An item is already in drag.");

            IndragButton = button;
        }

        /// <summary>
        /// Called by ItemButton when on down wasnt called but on release.
        /// </summary>
        public static void DragRelease(ItemButton targetButton)
        {
            if (IndragButton == null)
                throw new Exception("There is no itembutton in drag.");

            var packet = new ItemDragPacket
            {
                BeingDragID = IndragButton.Button.Index,
                BeingDragInterfaceID = IndragButton.Button.InterfaceId,
                DropOnID = targetButton.Button.Index,
                DropOnInterfaceID = targetButton.Button.InterfaceId
            };

            ClientCommunicator.Instance.WorldServerConnection.SendPacket(packet);

            IndragButton.DragEnded();
            IndragButton = null;
        }

        public static void CancelDrag()
        {
            IndragButton.DragEnded();
            IndragButton = null;
        }
    }
}
