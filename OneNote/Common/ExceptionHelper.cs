using System;
using System.Runtime.InteropServices;

namespace OneNoteApi.Common
{
    internal static class ExceptionHelper
    {
        public static OneNoteApiException TryToWrap(Exception e)
        {
            if (e is COMException cex)
            {
                var message = ErrorCodes.GetDescription(cex.ErrorCode);
                return new OneNoteApiException(message, cex);
            }

            return null;
        }
    }
}