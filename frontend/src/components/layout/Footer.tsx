import { Link } from "@tanstack/react-router";

export function Footer() {
  return (
    <footer className="mt-16 border-t border-border bg-muted/40">
      <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
        <div className="grid gap-10 md:grid-cols-4">
          <div>
            <p className="text-sm font-semibold tracking-tight text-foreground">GGHN Digital Learning</p>
            <p className="mt-2 text-sm text-muted-foreground">
              Strengthening health systems through knowledge, training, and research-driven learning across Africa.
            </p>
          </div>
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-foreground">Platform</p>
            <ul className="mt-3 space-y-2 text-sm text-muted-foreground">
              <li><Link to="/library" className="hover:text-foreground">Digital Library</Link></li>
              <li><Link to="/learning" className="hover:text-foreground">Courses</Link></li>
              <li><Link to="/pathways" className="hover:text-foreground">Learning Pathways</Link></li>
              <li><Link to="/templates" className="hover:text-foreground">Templates & Tools</Link></li>
            </ul>
          </div>
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-foreground">Organization</p>
            <ul className="mt-3 space-y-2 text-sm text-muted-foreground">
              <li>About GGHN</li>
              <li>Partnerships</li>
              <li>Research</li>
              <li>Contact</li>
            </ul>
          </div>
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-foreground">Legal</p>
            <ul className="mt-3 space-y-2 text-sm text-muted-foreground">
              <li>Privacy Policy</li>
              <li>Terms of Use</li>
              <li>Accessibility</li>
            </ul>
          </div>
        </div>
        <div className="mt-10 border-t border-border pt-6 text-xs text-muted-foreground">
          © {new Date().getFullYear()} Georgetown Global Health Nigeria. Affiliated with Georgetown University’s Center for Global Health Practice and Impact.
        </div>
      </div>
    </footer>
  );
}
