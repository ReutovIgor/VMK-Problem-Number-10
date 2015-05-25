<html>
 <head>
  <title>Пользователь</title>
 </head>
 <style type="text/css">
  .header
  {
  min-width: 1280px;
  max-width: 1600px;
  margin: 0 auto;
  padding: 0;
  position:absolute;
  top:0px;
  left:0px;
  background:#ccc;
  border: solid 1px black; 
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
    display: none
   }

   #form-container1 
   {
    display: none
   }
   </style>

   <style type="text/css">
   .block1 { 
    width: 250px; 
    background: #ccc;
    padding: 5px;
    padding-right: 20px; 
    border: solid 1px black; 
    position: absolute;
    top: 200px;
    left: 380px;
   }
   </style>

   <div id="header" class="header">
 <form style="header"><button href="#form-container" rel="get_consultatiom_win" id="button_get_consultation">Записаться на приём</button> <button href="#form-container1" rel="get_history_win" id="button_get_history">История</button>  <button style="header_button" name="button_get_online_consultation">Онлайн-консультация</button></form>
 </div>

<div id="form-container" class="block1">
<form id="get_consultation_win" name="Get_consultation_win">
    <p>Фамилия:<input type="text" name="surname" size="30"></p>
    <p>Имя:<input type="text" name="name" size="30"></p>
    <p>Пароль:<input type="password" name="password" size="30"></p>
    <p>Повторите:<input type="password" name="repeat_password" size="30"></p>
    <button name="apply">Подтвердить</button>
 </form>
 </div>

 <div id="form-container1" class="block1">
<form id="get_history_win" name="Get_history_win">
    <p>Фамилия:<input type="text" name="surname" size="30"></p>
    <p>Имя:<input type="text" name="name" size="30"></p>
    <p>Пароль:<input type="password" name="password" size="30"></p>
    <p>Повторите:<input type="password" name="repeat_password" size="30"></p>
    <button name="apply">Подтвердить</button>
 </form>
 </div>

 <body>
 <?php
 
 ?>
 <script>
document.getElementById('button_get_consultation').onclick=function()
{
  document.getElementById('form-container').style.display='block';
}

document.getElementById('button_get_history').onclick=function()
{
  document.getElementById('form-container1').style.display='block';
}
</script>
 </body>
</html>