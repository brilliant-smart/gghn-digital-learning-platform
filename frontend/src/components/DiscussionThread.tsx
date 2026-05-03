import { useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { MessageSquare, Reply, ChevronDown, ChevronUp, Trash2 } from "lucide-react";
import { discussionApi, type DiscussionDto } from "@/api/discussions";

interface DiscussionThreadProps {
  resourceId: string;
  discussions: DiscussionDto[];
  onRefresh: () => void;
  isAuthenticated: boolean;
  currentUserId?: string;
  isAdmin?: boolean;
}

export function DiscussionThread({ resourceId, discussions, onRefresh, isAuthenticated, currentUserId, isAdmin }: DiscussionThreadProps) {
  const [newComment, setNewComment] = useState("");
  const [replyingTo, setReplyingTo] = useState<string | null>(null);
  const [replyContent, setReplyContent] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [expandedReplies, setExpandedReplies] = useState<Set<string>>(new Set());

  const toggleReplies = (id: string) => {
    setExpandedReplies((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const handleSubmit = async () => {
    if (!newComment.trim()) return;
    setSubmitting(true);
    try {
      await discussionApi.create({ resourceId, content: newComment.trim() });
      setNewComment("");
      onRefresh();
    } catch (err) {
      console.error("Failed to post discussion", err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleReply = async (parentId: string) => {
    if (!replyContent.trim()) return;
    setSubmitting(true);
    try {
      await discussionApi.reply(parentId, { content: replyContent.trim() });
      setReplyContent("");
      setReplyingTo(null);
      onRefresh();
    } catch (err) {
      console.error("Failed to post reply", err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await discussionApi.delete(id);
      onRefresh();
    } catch (err) {
      console.error("Failed to delete discussion", err);
    }
  };

  const topLevelDiscussions = discussions.filter((d) => !d.parentId);

  return (
    <section className="mt-12">
      <h2 className="flex items-center gap-2 text-xl font-semibold tracking-tight text-foreground">
        <MessageSquare className="h-5 w-5" /> Discussion
      </h2>

      {isAuthenticated ? (
        <div className="mt-6 space-y-3">
          <Textarea
            placeholder="Share your thoughts or ask a question..."
            value={newComment}
            onChange={(e) => setNewComment(e.target.value)}
            rows={3}
          />
          <Button onClick={handleSubmit} disabled={submitting || !newComment.trim()}>
            Post comment
          </Button>
        </div>
      ) : (
        <p className="mt-4 text-sm text-muted-foreground">Sign in to join the discussion.</p>
      )}

      {topLevelDiscussions.length === 0 && (
        <p className="mt-6 text-sm text-muted-foreground">No comments yet. Be the first to share your thoughts!</p>
      )}

      <div className="mt-6 space-y-4">
        {topLevelDiscussions.map((d) => (
          <Card key={d.id}>
            <CardContent className="pt-4">
              <div className="flex items-start gap-3">
                <Avatar className="h-8 w-8">
                  <AvatarFallback className="text-xs">{d.userName.split(" ").map((n: string) => n[0]).join("").slice(0, 2)}</AvatarFallback>
                </Avatar>
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2">
                    <span className="text-sm font-medium text-foreground">{d.userName}</span>
                    <span className="text-xs text-muted-foreground">{new Date(d.createdAt).toLocaleDateString()}</span>
                  </div>
                  <p className="mt-1.5 text-sm text-foreground whitespace-pre-line">{d.content}</p>
                  <div className="mt-2 flex items-center gap-3">
                    {isAuthenticated && (
                      <Button variant="ghost" size="sm" className="h-7 text-xs" onClick={() => { setReplyingTo(replyingTo === d.id ? null : d.id); setReplyContent(""); }}>
                        <Reply className="mr-1 h-3 w-3" /> Reply
                      </Button>
                    )}
                    {(currentUserId === d.userId || isAdmin) && (
                      <Button variant="ghost" size="sm" className="h-7 text-xs text-destructive hover:text-destructive" onClick={() => handleDelete(d.id)}>
                        <Trash2 className="mr-1 h-3 w-3" /> Delete
                      </Button>
                    )}
                    {d.replies && d.replies.length > 0 && (
                      <Button variant="ghost" size="sm" className="h-7 text-xs" onClick={() => toggleReplies(d.id)}>
                        {expandedReplies.has(d.id) ? <ChevronUp className="mr-1 h-3 w-3" /> : <ChevronDown className="mr-1 h-3 w-3" />}
                        {d.replies.length} {d.replies.length === 1 ? "reply" : "replies"}
                      </Button>
                    )}
                  </div>
                  {replyingTo === d.id && (
                    <div className="mt-3 space-y-2">
                      <Textarea placeholder="Write a reply..." value={replyContent} onChange={(e) => setReplyContent(e.target.value)} rows={2} />
                      <div className="flex gap-2">
                        <Button size="sm" onClick={() => handleReply(d.id)} disabled={submitting || !replyContent.trim()}>Reply</Button>
                        <Button size="sm" variant="outline" onClick={() => setReplyingTo(null)}>Cancel</Button>
                      </div>
                    </div>
                  )}
                  {d.replies && d.replies.length > 0 && expandedReplies.has(d.id) && (
                    <div className="mt-4 space-y-3 border-l-2 border-border pl-4">
                      {d.replies.map((r) => (
                        <div key={r.id} className="flex items-start gap-3">
                          <Avatar className="h-7 w-7">
                            <AvatarFallback className="text-[10px]">{r.userName.split(" ").map((n: string) => n[0]).join("").slice(0, 2)}</AvatarFallback>
                          </Avatar>
                          <div className="flex-1 min-w-0">
                            <div className="flex items-center gap-2">
                              <span className="text-sm font-medium text-foreground">{r.userName}</span>
                              <span className="text-xs text-muted-foreground">{new Date(r.createdAt).toLocaleDateString()}</span>
                            </div>
                            <p className="mt-1 text-sm text-foreground whitespace-pre-line">{r.content}</p>
                            {(currentUserId === r.userId || isAdmin) && (
                              <Button variant="ghost" size="sm" className="mt-1 h-6 text-xs text-destructive hover:text-destructive" onClick={() => handleDelete(r.id)}>
                                <Trash2 className="mr-1 h-3 w-3" /> Delete
                              </Button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </section>
  );
}