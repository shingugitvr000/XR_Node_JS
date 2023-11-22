const WebSocket = require('ws');
var CREATE = require('./create.js'); // 방이 만들어졌을떄 호출 하는 클래스

const wss = new WebSocket.Server({ port:8000} , () => {         //소캣서버을 포트 8000번에 시작 시킨다. 
    console.log('서버 시작');
});

const userList = [];
const maxClients = 5;   //최대 접속 인원수
let rooms = {};         //룸 배열
let joinuserTemp = 1;   //유저 구분 인자

wss.on('connection' , function connections(ws){         //커넥션 이 되었을 때
    ws.clientID = genKey(8);

    var create = new CREATE();      //방 생성 객체를 new 로 선언한다. 

    ws.on('message' , (data) => {
        const jsonData = JSON.parse(data);

        let requestType = jsonData.requestType;         //리퀘스트 타입으로 결정
        let params = jsonData.message;                  //파라미터 추가

        console.log('받은 데이터 : ' , jsonData , requestType , params);

        if(requestType == 10)       //유저 리스트 
        {
            ws.send(JSON.stringify(userList));
        }

        if(requestType == 100)      //방생성 
        {
           create.createRoom(params, rooms, ws);
        }
        if(requestType == 200)      //방 입장   
        {
           joinRoom(params,ws);
        }
        if(requestType == 300)      //방 퇴실   
        {
            leaveRoom(params);
        }

        if(requestType == 0)        //전체 에코
        {
            wss.clients.forEach((client)=>
            {
                client.send(data); //받은 데이터를 모든 클라이언트에게 전송
            })
        }
       
    });

    ws.on('close', ()=> {
        const index = userList.indexOf(ws.clientID);
        if(index !== -1)
        {
            console.log('클라이언트가 해제됨 - ID : ' , ws.clientID);
            userList.splice(index, 1);      //배열에서 해당 클라이언트 제거
        }
    });
    //새로 연결된 클라이언트를 유저 리스트에 추가
    userList.push(ws.clientID);
    //클라이언트에게 임시 유저 이름 전송
    ws.send(JSON.stringify({clientID : ws.clientID}));
    //연결된 클라이언트 유저 이름 로그 출력
    console.log('클라이언트 연결 - ID :' , ws.clientID);
})

function generalInformation(ws) {
    
    let obj;

    if(ws["room"] != undefined)
    {
        obj = {
            "type" : "info" ,
            "params" : {
                "room" : ws["room"],
                "no-clinets" : rooms[ws["room"]].length,
            }
        }
    }
    else
    {
        obj = {
            "type" : "info" ,
            "params" : {
                "room" : "no room",
            }
        }
    }

    ws.send(JSON.stringify(obj));
}

function joinRoom(params , ws)
{
    const room = params;
    if(!Object.keys(rooms).includes(room))
    {
        console.warn(room + ' does net exist');  //룸이 없다는 경고 콘솔
        return;
    }

    if(rooms[room].length >= maxClients) {      //5명 이상 못들어가게 막는 라인
        console.warn(room + ' is full');  //룸이 없다는 경고 콘솔
        return;
    }

    rooms[room].push(ws);
    ws["room"] = room;

    generalInformation(ws);

    var UserList = "";

    for(let i = 0 ; i < rooms[room].length; i++)
    {
        UserList += "User : " + rooms[room][i].user + " \n";
    }
    joinuserTemp += 1;

    obj = {
        "type" : "info",
        "myParams" : {
            "room" : ws["room"],
            "UserList" : UserList
        }
    }

    for(var i = 0 ; i < rooms[room].length; i++)
    {
        rooms[room][i].send(JSON.stringify(obj));
    }

}

function leaveRoom(params) //룸을 나갈경우
{
    const room = ws.room;

    if(rooms[room].length > 0)
    {
        rooms[room] = rooms[room].filter(so => so !== ws);

        ws["room"] = undefined;

        if(rooms[room].length == 0)     //룸에 0명이 되었을 때
        {
            close(room);
        }
    }

    function close(room) {  //룸을 게거한다. 
        if(rooms.length > 0)
        rooms = rooms.filter(key => key !== room);
    }
}


wss.on('listening' , () => {
    console.log('리스닝....');
})

function genKey(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';

    for(let i = 0 ; i < length; i++)
    {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return result;
}