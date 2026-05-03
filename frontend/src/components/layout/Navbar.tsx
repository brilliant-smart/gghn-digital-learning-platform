import { Link } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Menu, LogOut, User, LayoutDashboard, Shield, ChevronDown } from "lucide-react";
import { useState } from "react";
import gghnLogo from "@/assets/gghn-logo.png";
import { useAuth } from "@/lib/auth";

const links = [
  { to: "/", label: "Home" },
  { to: "/library", label: "Library" },
  { to: "/learning", label: "Learning" },
  { to: "/pathways", label: "Pathways" },
  { to: "/publications", label: "Publications" },
  { to: "/templates", label: "Resources" },
  { to: "/conference", label: "Conference" },
] as const;

export function Navbar() {
  const [open, setOpen] = useState(false);
  const { user, isAuthenticated, logout } = useAuth();
  const isAdminOrEditor = user?.roles?.some(r => r === "Admin" || r === "Editor");
  const isAdmin = user?.roles?.includes("Admin");

  return (
    <header className="sticky top-0 z-40 w-full border-b border-border bg-background/85 backdrop-blur supports-[backdrop-filter]:bg-background/70">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <Link to="/" className="flex items-center gap-2">
          <img src={gghnLogo} alt="GGHN" className="h-8 w-auto" />
          <span className="hidden text-[11px] text-muted-foreground sm:inline">Digital Learning</span>
        </Link>

        <nav className="hidden items-center gap-1 md:flex">
          {links.map((l) => (
            <Link
              key={l.to}
              to={l.to}
              className="rounded-md px-3 py-2 text-sm font-medium text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
              activeProps={{ className: "rounded-md px-3 py-2 text-sm font-medium bg-primary-soft text-primary" }}
              activeOptions={{ exact: l.to === "/" }}
            >
              {l.label}
            </Link>
          ))}
        </nav>

        <div className="hidden items-center gap-1 md:flex">
          {isAuthenticated ? (
            <>
              <Link to="/dashboard" title="Dashboard">
                <Button variant="ghost" size="icon" className="h-9 w-9">
                  <LayoutDashboard className="h-4 w-4" />
                </Button>
              </Link>
              {isAdminOrEditor && (
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" size="icon" className="h-9 w-9" title="Admin">
                      <Shield className="h-4 w-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem asChild>
                      <Link to="/admin/editorial">Editorial Queue</Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem asChild>
                      <Link to="/admin/conferences">Conference Mgmt</Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem asChild>
                      <Link to="/admin/registrations">Registrations</Link>
                    </DropdownMenuItem>
                    {isAdmin && (
                      <DropdownMenuItem asChild>
                        <Link to="/admin/users">User Management</Link>
                      </DropdownMenuItem>
                    )}
                    <DropdownMenuItem asChild>
                      <Link to="/admin/analytics">Analytics</Link>
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              )}
              <Link to="/profile" title="Profile">
                <Button variant="ghost" size="icon" className="h-9 w-9">
                  <User className="h-4 w-4" />
                </Button>
              </Link>
              <Button variant="ghost" size="icon" className="h-9 w-9" onClick={logout} title="Sign out">
                <LogOut className="h-4 w-4" />
              </Button>
            </>
          ) : (
            <Link to="/auth">
              <Button size="sm">Login / Register</Button>
            </Link>
          )}
        </div>

        <button
          aria-label="Open menu"
          className="inline-flex items-center justify-center rounded-md p-2 text-foreground md:hidden"
          onClick={() => setOpen((o) => !o)}
        >
          <Menu className="h-5 w-5" />
        </button>
      </div>

      {open && (
        <div className="border-t border-border bg-background md:hidden">
          <div className="space-y-1 px-4 py-3">
            {links.map((l) => (
              <Link
                key={l.to}
                to={l.to}
                onClick={() => setOpen(false)}
                className="block rounded-md px-3 py-2 text-sm font-medium text-muted-foreground hover:bg-muted hover:text-foreground"
                activeProps={{ className: "block rounded-md px-3 py-2 text-sm font-medium bg-primary-soft text-primary" }}
                activeOptions={{ exact: l.to === "/" }}
              >
                {l.label}
              </Link>
            ))}
            <div className="flex gap-2 pt-2">
              {isAuthenticated ? (
                <>
                  <Link to="/dashboard" className="flex-1" onClick={() => setOpen(false)}>
                    <Button variant="outline" size="sm" className="w-full gap-1.5">
                      <LayoutDashboard className="h-3.5 w-3.5" /> Dashboard
                    </Button>
                  </Link>
                  {isAdminOrEditor && (
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="outline" size="sm" className="flex-1 gap-1.5">
                          <Shield className="h-3.5 w-3.5" /> Admin <ChevronDown className="h-3 w-3" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="start">
                        <DropdownMenuItem asChild>
                          <Link to="/admin/editorial" onClick={() => setOpen(false)}>Editorial Queue</Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                          <Link to="/admin/conferences" onClick={() => setOpen(false)}>Conference Mgmt</Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                          <Link to="/admin/registrations" onClick={() => setOpen(false)}>Registrations</Link>
                        </DropdownMenuItem>
                        {isAdmin && (
                          <DropdownMenuItem asChild>
                            <Link to="/admin/users" onClick={() => setOpen(false)}>User Management</Link>
                          </DropdownMenuItem>
                        )}
                        <DropdownMenuItem asChild>
                          <Link to="/admin/analytics" onClick={() => setOpen(false)}>Analytics</Link>
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  )}
                  <Link to="/profile" className="flex-1" onClick={() => setOpen(false)}>
                    <Button variant="outline" size="sm" className="w-full gap-1.5">
                      <User className="h-3.5 w-3.5" /> Profile
                    </Button>
                  </Link>
                  <Button size="sm" variant="ghost" onClick={() => { logout(); setOpen(false); }} title="Sign out">
                    <LogOut className="h-4 w-4" />
                  </Button>
                </>
              ) : (
                <Link to="/auth" className="flex-1" onClick={() => setOpen(false)}>
                  <Button size="sm" className="w-full">Login</Button>
                </Link>
              )}
            </div>
          </div>
        </div>
      )}
    </header>
  );
}