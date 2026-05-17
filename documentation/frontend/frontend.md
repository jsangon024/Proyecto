# Meal Planner Frontend

Aplicacion React con Vite separada del backend .NET.

## Estructura

```text
src/
  api/          Clientes HTTP por recurso
  components/   Componentes reutilizables
  config/       Variables de entorno
  hooks/        Estado compartido y acciones asincronas
  pages/        Pantallas principales
  utils/        Utilidades compartidas
```

## Pantallas

- Login y registro.
- Activacion de cuenta por enlace de correo.
- Recuperacion de password por correo.
- Panel principal.
- Ingredientes.
- Recetas con catalogo, creacion, edicion y detalle.
- Inventario.
- Plan mensual con objetivos nutricionales, alimentos prohibidos, planes guardados y lista de compra.
- Ajustes de cuenta.
- Administracion de usuarios.

## Navegacion

La aplicacion usa rutas hash para mantener la pantalla al recargar:

- `#/dashboard`
- `#/login`
- `#/verify-email/{token}`
- `#/reset-password/{token}`
- `#/ingredients`
- `#/recipes`
- `#/recipes/{id}`
- `#/inventory`
- `#/planner`
- `#/settings`
- `#/admin`

## Configuracion

Archivo `.env`:

```text
VITE_API_BASE_URL=http://localhost:8080
```

Usa `http://localhost:8080` con Docker y `http://localhost:5088` si ejecutas la API con `dotnet run`.

El registro no inicia sesion automaticamente. Tras crear la cuenta, el usuario debe abrir el enlace de activacion recibido por correo.

En Gestion del plan, el usuario puede marcar alimentos prohibidos desde el catalogo global de ingredientes. Esa seleccion se guarda en su perfil y se envia a la API junto con sus objetivos nutricionales.

## Ejecutar

```powershell
cd frontend
npm install
npm run dev
```

Abrir:

```text
http://localhost:5173
```

## Build

```powershell
npm run build
```
