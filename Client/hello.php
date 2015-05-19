<html>
 <head>
  <title>SuperMed вход</title>
 </head>
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
   <style>
   #form-container 
   {
   	display: none
   }
   </style>
   <div id="form-container" class="block1">
<form id="regwin" name="registration">
  	<p>Фамилия:<input type="text" name="surname" size="30"></p>
  	<p>Имя:<input type="text" name="name" size="30"></p>
  	<p>Пароль:<input type="password" name="password" size="30"></p>
  	<p>Повторите:<input type="password" name="repeat_password" size="30"></p>
  	<button name="apply">Подтвердить</button>
 </form>
 </div>
 <body>
 <?php
  echo '<p align="center"> <font size="50">Super <img src="images.jpg" width="50" height="50"> Med </font> </p>';
  echo '<p align="center">Логин:<input type="text" name="Login" value="" size="20"></p>';
  echo '<p align="center">Пароль:<input type="password" name="Password" value="" size="20"></p>';
  echo '<p align="center"><button name="enter">Войти</button> <a style="padding-left:20px" href="#form-container" id="sign_up" rel="regwin">Зарегестрироваться!</a> </p>';
 ?>
<script>
document.getElementById('sign_up').onclick=function()
{
	document.getElementById('form-container').style.display='block';
}
</script>
 </body>
</html>