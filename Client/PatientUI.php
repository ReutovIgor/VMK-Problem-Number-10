<html>
 <head>
  <title>Пользователь</title>
 </head>
 <style type="text/css">
  .header1
  {
  width: 226px;
  position:absolute;
  top:0px;
  left:0px;
  background:#ccc;
  border: solid 1px black; 
  text-align: center;
}

.header2
  {
  width: 200px;
  position:absolute;
  top:0px;
  left:228px;
  background:#ccc;
  border: solid 1px black; 
  text-align: center;
}

.header3
  {
  width: 200px;
  position:absolute;
  top:0px;
  left:430px;
  background:#ccc;
  border: solid 1px black; 
  text-align: center;
}
 </style>
 <style>
 .header_button
 {
    height: 50px;
 }
 </style>

 <style>
 #form-container 
   {
    display: none;
   }

   #form-container2 
   {
    display: none;
   }
   </style>

   <style type="text/css">
   .block1 { 
    width: 201px; 
    background: #ccc;
    padding: 5px;
    padding-right: 20px; 
    border: solid 1px black; 
    position: absolute;
    top: 35px;
    left: 0px;
   }

   .block2 { 
    width: 175px; 
    background: #ccc;
    padding: 5px;
    padding-right: 20px; 
    border: solid 1px black; 
    position: absolute;
    top: 35px;
    left: 228px;
    text-align: center;
   }
   </style>

   <div id="header" class="header1">
 <form><a href="#form-container" rel="get_consultatiom_win" id="button_get_consultation">Записаться на приём</a> </form>
 </div>

 <div id="header" class="header2">
 <form><a href="#form-container" rel="get_history_win" id="button_get_history">История</a> </form>
 </div>

 <div id="header" class="header3">
 <form><a href="#form-container" rel="get_history_win" id="button_get_history">Онлайн-консультация</a> </form>
 </div>

 <div id="form-container2" class="block2">
<form id="get_history_win" name="Get_history_win">
<p>Здесь будет отображаться история ваших действий</p>
</form>
</div>

<div id="form-container" class="block1">
<form id="get_consultation_win" name="Get_consultation_win">
    <p>
    Выберите филиал: 
    <select>
      <option>Нижний Новгород</option>
      <option>Дзержинск</option>
      <option>Муром</option>
      <option>Правдинск</option>
    </select>
    </p>

    <p>
    Укажите направление:
    <select>
      <option> </option>
      <option>Кардиология</option>
      <option>Диетология</option>
      <option>Ортопедия</option>
    </select>
    </p>

    <p>
    Уточните специалиста:
    <select>
     <option> </option>
     <option>Нижний Новгород</option>
    </select>
    </p>

    <p>
    Выберите время:
    <select>
    <option> </option>
    <option>Нижний Новгород</option>
    </select>
    </p>

    
    <button name="apply">Подтвердить</button>
 </form>
 </div>


 
 <script>
document.getElementById('button_get_consultation').onclick=function()
{
  document.getElementById('form-container').style.display='block';
}

document.getElementById('button_get_history').onclick=function()
{
  document.getElementById('form-container2').style.display='block';
}
</script>

</html>