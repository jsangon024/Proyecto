export function PageHeader({ eyebrow, title, children }) {
  return (
    <header className="page-header">
      <div>
        <p>{eyebrow}</p>
        <h2>{title}</h2>
      </div>
      {children}
    </header>
  );
}
