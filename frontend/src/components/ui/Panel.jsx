export function Panel({ title, icon, children, className = '' }) {
  return (
    <article className={`panel ${className}`}>
      {title && (
        <h3>
          {icon}
          {title}
        </h3>
      )}
      {children}
    </article>
  );
}
