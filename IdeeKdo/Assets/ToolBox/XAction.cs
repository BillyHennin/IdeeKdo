using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Android.Util;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;

namespace IdeeKdo.Assets.ToolBox
{
    public static class XAction
    {
        /// <summary>
        ///     Liste d'action � effectuer
        /// </summary>
        private static List<ActionStack> _actionsToExecute;

        /// <summary>
        ///     Initialise la liste d'action et active l'evenement de changement de connection
        /// </summary>
        private static void Init()
        {
            _actionsToExecute = new List<ActionStack>();
            CrossConnectivity.Current.ConnectivityChanged += ConnectivityChangedEvent;
        }

        /// <summary>
        ///     Fonction appel�e lorsque l'�tat de connection change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnectivityChangedEvent(object sender, ConnectivityChangedEventArgs e)
        {
            var isConnected = CrossConnectivity.Current.IsConnected;
            var strMessage = $"La connection � �t� {(isConnected ? "retrouv�e" : "perdue")}.";
            XLog.Write(LogPriority.Info, strMessage);
            if (isConnected)
            {
                //Affiche un message en bas de l'�cran indiquant que la connection est revenue
                Xui.SetSnackMessage(strMessage);
                //Effectuer toutes les actions mise en attente
                ExecuteStack();
            }
            else
            {
                XMessage.ShowNotification(strMessage,
                    "Toute action sera effectu�e � la reprise de la connection");
            }
        }

        /// <summary>
        ///     Permet d'ajouter une fonction au stack d'attente
        /// </summary>
        /// <param name="action">Fonction � ajouter � la pile</param>
        public static void AddToStack(Action<object> action)
        {
            if (_actionsToExecute.Equals(null))
            {
                Init();
            }
            _actionsToExecute.Add(new ActionStack(action));
        }

        /// <summary>
        ///     Enleve tous les elements de la pile qui correspondent � la m�thode pass�e en parametre.
        ///     Si aucun parametre n'est utilis�, la pile est entierement vid�e
        /// </summary>
        /// <param name="action">Fonction � supprimer de la pile</param>
        public static void RemoveFromStackAll(Action<object> action = null)
        {
            if (action == null)
            {
                _actionsToExecute.Clear();
            }
            else
            {
                _actionsToExecute.RemoveAll(x => x.Method == action);
            }
        }

        /// <summary>
        ///     Enleve le premier element de la pile qui correspond � la m�thode pass�e en parametre
        /// </summary>
        /// <param name="action">Fonction � supprimer de la pile</param>
        public static void RemoveFromStack(Action<object> action)
            => _actionsToExecute.Remove(_actionsToExecute.Find(x => x.Method == action));

        /// <summary>
        ///     Enleve le dernier element de la pile
        /// </summary>
        public static void RemoveLast()
            => _actionsToExecute.RemoveAt(_actionsToExecute.Count - 1);

        /// <summary>
        ///     Effectue toutes les fonctions en attente
        /// </summary>
        private static void ExecuteStack()
        {
            var listActions = new List<ActionStack>();
            foreach (var action in _actionsToExecute)
            {
                //On empeche les fonctions STRICTEMENT identiques de se lancer plusieurs fois.
                //Si une fonction apparait deux fois dans la liste, mais qu'elle possede des valeurs de parametres differents, elle sera execut�e.
                if (listActions.Any(x => x.Method == action.Method))
                {
                    continue;
                }
                action.Execute();
                listActions.Add(action);
            }
            //On enleve toutes les fonctions qui ont �t� effectu�es de la liste
            _actionsToExecute.RemoveAll(method => method.BToRemove);
        }
    }

    public class ActionStack
    {
        public readonly Action<object> Method;
        public bool BToRemove;

        public ActionStack(Action<object> method)
        {
            Method = method;
        }

        /// <summary>
        ///     Lance la fonction et indique si elle � �t� effectu�e ou non.
        /// </summary>
        public void Execute()
        {
            try
            {
                if (BToRemove)
                {
                    return;
                }
                Method.Invoke(null);
                BToRemove = true;
            }
            catch
            {
                var error = $"La m�thode '{Method.GetMethodInfo().Name}' a rencontr�e une erreur lors de son execution.";
                XLog.Write(LogPriority.Error, error);
                Xui.SetSnackMessage(error);
            }
        }
    }
}