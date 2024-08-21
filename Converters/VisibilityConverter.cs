using KoriApp.DB.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoriApp.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        private string GetCurrentUserId()
        {
            var auth = FirebaseConnection.GetCurrentUser();
            var currentUser = auth.User;
            return currentUser?.Uid;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Obtén el ID del usuario actual
            var currentUserId = GetCurrentUserId();

            if (string.IsNullOrEmpty(currentUserId))
            {
                return false; // Ocultar el botón si no hay un usuario autenticado
            }

            // Compara el CreatorID de la publicación con el ID del usuario actual
            if (value is string creatorId && creatorId == currentUserId)
            {
                return true; // Mostrar el botón
            }

            return false; // Ocultar el botón
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
