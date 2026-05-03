import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Shield, Users } from "lucide-react";
import { userApi } from "@/api/users";
import { useAuth } from "@/lib/auth";
import { useNavigate } from "@tanstack/react-router";
import { type UserDto } from "@/api/auth";
import { type PagedResult } from "@/api/resources";

export const Route = createFileRoute("/admin/users")({
  head: () => ({ meta: [{ title: "User Management | GGHN Admin" }] }),
  component: UserManagementPage,
});

const ROLES = ["Admin", "Editor", "Member", "Institutional", "FreeUser"];
const TIERS = ["Free", "Member", "Institutional"];

function UserManagementPage() {
  const { user, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [users, setUsers] = useState<PagedResult<UserDto>>({ items: [], totalCount: 0, page: 1, pageSize: 20, totalPages: 1 });
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);

  useEffect(() => {
    if (!isAuthenticated || !user?.roles?.includes("Admin")) {
      navigate({ to: "/dashboard", replace: true });
      return;
    }
    userApi.getAll(page, 20)
      .then(setUsers)
      .catch(() => {})
      .finally(() => setLoading(false));
  }, [isAuthenticated, page]);

  const handleRoleChange = async (userId: string, role: string) => {
    try {
      await userApi.updateRole(userId, role);
      setUsers(u => ({ ...u, items: u.items.map(usr => usr.id === userId ? { ...usr, roles: [role] } : usr) }));
    } catch (err) { console.error(err); }
  };

  const handleTierChange = async (userId: string, tier: string) => {
    try {
      await userApi.updateTier(userId, tier);
      setUsers(u => ({ ...u, items: u.items.map(usr => usr.id === userId ? { ...usr, membershipTier: tier } : usr) }));
    } catch (err) { console.error(err); }
  };

  const handleDelete = async (userId: string) => {
    if (!confirm("Are you sure you want to delete this user?")) return;
    try {
      await userApi.deleteUser(userId);
      setUsers(u => ({ ...u, items: u.items.filter(usr => usr.id !== userId), totalCount: u.totalCount - 1 }));
    } catch (err) { console.error(err); }
  };

  if (loading) return <SiteShell><div className="flex justify-center py-20"><p className="text-muted-foreground">Loading...</p></div></SiteShell>;

  return (
    <SiteShell>
      <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="flex items-center gap-3 mb-8">
          <Users className="h-6 w-6 text-primary" />
          <h1 className="text-2xl font-bold tracking-tight">User Management</h1>
          <Badge variant="secondary" className="ml-2">{users.totalCount} users</Badge>
        </div>
        <Card>
          <CardContent className="p-0">
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead className="border-b bg-muted/50">
                  <tr>
                    <th className="px-4 py-3 text-left font-medium">Name</th>
                    <th className="px-4 py-3 text-left font-medium">Email</th>
                    <th className="px-4 py-3 text-left font-medium">Tier</th>
                    <th className="px-4 py-3 text-left font-medium">Role</th>
                    <th className="px-4 py-3 text-left font-medium">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {users.items.map((u) => (
                    <tr key={u.id} className="border-b last:border-0">
                      <td className="px-4 py-3 font-medium">{u.firstName} {u.lastName}</td>
                      <td className="px-4 py-3 text-muted-foreground">{u.email}</td>
                      <td className="px-4 py-3">
                        <Select value={u.membershipTier} onValueChange={(v) => handleTierChange(u.id, v)}>
                          <SelectTrigger className="h-8 w-[130px]"><SelectValue /></SelectTrigger>
                          <SelectContent>{TIERS.map(t => <SelectItem key={t} value={t}>{t}</SelectItem>)}</SelectContent>
                        </Select>
                      </td>
                      <td className="px-4 py-3">
                        <Select value={u.roles[0] || "FreeUser"} onValueChange={(v) => handleRoleChange(u.id, v)}>
                          <SelectTrigger className="h-8 w-[130px]"><SelectValue /></SelectTrigger>
                          <SelectContent>{ROLES.map(r => <SelectItem key={r} value={r}>{r}</SelectItem>)}</SelectContent>
                        </Select>
                      </td>
                      <td className="px-4 py-3">
                        <Button variant="outline" size="sm" className="text-destructive hover:text-destructive" onClick={() => handleDelete(u.id)}>Delete</Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardContent>
        </Card>
        {users.totalPages > 1 && (
          <div className="mt-4 flex items-center justify-center gap-2">
            <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>Previous</Button>
            <span className="text-sm text-muted-foreground">Page {page} of {users.totalPages}</span>
            <Button variant="outline" size="sm" disabled={page >= users.totalPages} onClick={() => setPage(p => p + 1)}>Next</Button>
          </div>
        )}
      </div>
    </SiteShell>
  );
}