using System.Text;
using WebApi;
var db = new DB();
var records =db.Data();
var u8 = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/{id}", (string id) => {
    int _id;
    if(int.TryParse(id, out _id))
    {
        if(_id >= 0 && _id <= records.Count - 1)
        {
            return db.TryGet(records[_id].linkId, records[_id].clcId, records[_id].sbp);
        }
    }
    return new Result { status = -1024, value = "", message = "Invalid input." };
});

app.MapPut("/{id}", (string id) =>{
    int _id;
    if (int.TryParse(id, out _id))
    {
        if (_id >= 0 && _id <= records.Count - 1)
        {
            return db.TrySet(records[_id].linkId, records[_id].clcId, records[_id].sbp, records[_id].url);
        }
    }
    return new Result { status = -1024, value = "", message = "Invalid input." };
});

app.MapDelete("/{id}", (string id) =>{
    int _id;
    if (int.TryParse(id, out _id))
    {
        if (_id >= 0 && _id <= records.Count - 1)
        {
            return db.TryDelete(records[_id].linkId, records[_id].clcId, records[_id].sbp);
        }
    }
    return new Result { status = -1024, value = "", message = "Invalid input." };
});

app.Run();
