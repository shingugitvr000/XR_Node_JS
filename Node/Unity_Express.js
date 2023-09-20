const express = require('express');
const app = express();

let users = [
    { id: 0, data: "UserData"}              //배열안에 오브젝트
];

app.use(express.json());                   //json을 사용하겠다.

app.get('/',(req ,res)=>{

    let result = {
        cmd : -1,
        message : 'Hello world'
    };

    res.send(result);
});

app.get('/id',(req ,res)=>{

    let result = {
        cmd : 100,
        message : 'MyID'
    };

    res.send(result);
});

app.get('/userdata/list' , (req , res) => {         //유저 리스트 반환

    let result = users.sort(function(a,b){          //ID 순서로 정렬
            return b.id - a.id;
    });

    result = result.slice(0, users.length);         //유저 숫자만큼 배열 생성 0 ~ n-1

    res.send({                                      //패킷을 완성해서 보낸다. 
        cmd : 1101,
        message : '',
        result
    });

});

app.post('/userdata' , (req ,res) => {
    const{id, data} = req.body;                 //Reqest Body에 Json 데이터가 들어오면 id, data에 연결
    console.log(id,data);
    let result = {
        cmd: -1,
        message: ''
    };

    let user = users.find(x=>x.id == id);       //users 배열에서 X를 찾아서 리턴

    if(user == undefined)                       //undefined 될경우 신규 등록
    {
        users.push({id , data});                //배열에 입력 Push
        result.cmd = 1001;
        result.message = "유저 신규 등록";
    }
    else
    {
        console.log(id, user.data);
        user.data = data;
        result.cmd = 1002;
        result.message = '데이터 갱신';
    }

    console.log(users);
    res.send(result);

});

app.listen(3030, ()=> {
    console.log('server is running at 3030 port');
});