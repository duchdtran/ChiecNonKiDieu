using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    enum Opcode
    {
        //client
        JOIN_ROOM = 101,
        JOIN_PLAY = 102,
        SPIN = 111,
        CLICK_KEYWORK = 112,
        CLICK_HINT = 113,
        EXIT = 132,

        //server
        CREATE_ROOM_SUCCESS,
        JOIN_ROOM_SUCCESS,
        JOIN_ROOM_ERROR,
        START_GAME,
        SEND_QUESTION,
        SEND_KEYWORD,
        SEND_HINT,
        FALSE_FRENCH,
        CLOSE_SOCKET
    }
}
