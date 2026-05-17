export function confirmDelete(targetName) {
  return window.confirm(`Vas a eliminar ${targetName}. Esta accion no se puede deshacer. ¿Quieres continuar?`);
}
