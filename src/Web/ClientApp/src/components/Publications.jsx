import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

export function Publications() {
  const [query, setQuery] = useState("");
  const [result, setResult] = useState({ publications: [], total: 0 });
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    const url = new URL("/api/publications", window.location.origin);
    if (query) url.searchParams.set("q", query);
    setLoading(true);
    fetch(url).then(r => r.ok ? r.json() : Promise.reject()).then(setResult).catch(() => setResult({ publications: [], total: 0 })).finally(() => setLoading(false));
  }, [query]);
  return <section className="portal-publications" aria-labelledby="publications-title">
    <header><p className="eyebrow">Portal público institucional</p><h1 id="publications-title">Publicações institucionais</h1><p>Notícias, comunicados, atas, eventos e informações da associação.</p></header>
    <form role="search" className="publication-search" onSubmit={event => { event.preventDefault(); setQuery(new FormData(event.currentTarget).get("q") || ""); }}>
      <label htmlFor="publication-query">Pesquisar publicações</label><div><input id="publication-query" name="q" type="search" placeholder="Digite um assunto"/><button type="submit">Pesquisar</button></div>
    </form>
    <p aria-live="polite">{loading ? "Carregando publicações…" : `${result.total} publicação(ões) encontrada(s).`}</p>
    {!loading && result.publications.length === 0 ? <p role="status">Nenhuma Publicação institucional encontrada. Tente outros termos.</p> : <div className="publication-grid">{result.publications.map(item => <article key={item.id}><header><small>{item.category}</small><h2><Link to={`/publicacoes/${item.id}`}>{item.title}</Link></h2></header><p>{item.summary}</p><footer><time dateTime={item.publishedAt}>Publicado em {new Date(item.publishedAt).toLocaleDateString("pt-BR")}</time></footer></article>)}</div>}
  </section>;
}

export function PublicationDetail() {
  const id = window.location.pathname.split("/").pop();
  const [publication, setPublication] = useState(null); const [failed, setFailed] = useState(false);
  useEffect(() => { fetch(`/api/publications/${id}`).then(r => r.ok ? r.json() : Promise.reject()).then(setPublication).catch(() => setFailed(true)); }, [id]);
  if (failed) return <section><h1>Publicação não encontrada</h1><p>Este conteúdo não está disponível publicamente.</p><Link to="/publicacoes">Voltar às publicações</Link></section>;
  if (!publication) return <p aria-live="polite">Carregando publicação…</p>;
  return <article className="publication-detail"><header><p className="eyebrow">{publication.category}</p><h1>{publication.title}</h1><p>{publication.summary}</p><time dateTime={publication.publishedAt}>Publicado em {new Date(publication.publishedAt).toLocaleDateString("pt-BR")}</time></header><div className="publication-body">{publication.body}</div><footer><Link to="/publicacoes">Voltar às publicações</Link></footer></article>;
}
