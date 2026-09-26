# InterClini

Plataforma de interoperabilidad clínica basada en **HL7 FHIR R4**, construida con **.NET 10** y **Clean Architecture**.

## ¿Qué hace?

InterClini expone una API REST simple para aplicaciones clínicas (móviles o web) y por debajo habla FHIR R4 contra servidores compatibles. Esto permite que las apps cliente no tengan que conocer los detalles del estándar FHIR.

## Arquitectura

El proyecto sigue Clean Architecture con 5 capas:
