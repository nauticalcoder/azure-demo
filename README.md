Azure demo repo used to allow Azure support to look at an issue with the task sometimes running and sometimes failing to run.

It turns out our deployment was missing a dll, `zlib` and was picking up a different version from the Azure server.
