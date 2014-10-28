// Do funny backwards initialisation. If we're *not* a dedicated client, we need
// client scripts, and vice versa. If we're not a dedicated anything, we need
// both client and server scripts.
if ($Game::argv[1] !$= client) exec("tutorials/client_server/server.cs");
if ($Game::argv[1] !$= server) exec("tutorials/client_server/client.cs");
