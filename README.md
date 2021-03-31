# Simple-Chat-Solution

Este projeto tem como objetivo implementar um chat simples, onde um servidor (Console App) pode receber conexões locais de multiplos clientes.

Arquitetura:
Servidor = Console Aplication criado em .NET Framework 4.7.2
Client = Windows Aplication criado em .NET Core 3.0

Alguns comandos foram implementados.

Para executar os comandos, basta digitar no local específicado para chat e clicar em Send ou apertar Enter.
/exit -> fecha a janela do ClientChat.
/logout -> desloga do Servidor.
/changeNick {NovoNick} -> altera o nick do usuario.
/join {nomeDaSala} -> o usuario entrará em uma sala, saindo da anterior.
/w {NickDoDestinatario} {mensagem} -> O usuario envia uma mensagem para outro usuario, independente deste estar na mesma sala ou não.
/help -> O usuario recebe ajuda do servidor.

# Algumas imagens do sistema.

# Foto 1
![Foto1](./Photos/Usuario-Conectado.png)

# Foto 2
![Foto2](./Photos/multiplosClientsConectados.png)

# Para executar o sistema.

Basta copiar o código para o visual studio e executar os projetos simple_chat_client e simple_chat_server.

