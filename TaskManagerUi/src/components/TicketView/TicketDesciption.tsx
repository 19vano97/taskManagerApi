import { RichTextEditor } from '@mantine/tiptap';
import type { Editor } from '@tiptap/react';
import { useEffect } from 'react';

export type TicketDesciptionProps = {
    editor: Editor | null;
    content: string;
    onChange: (content: string) => void;
};

export const TicketDesciption = ({ editor, content, onChange }: TicketDesciptionProps) => {
    useEffect(() => {
        if (editor) {
            editor.commands.setContent(content || ''); // Ensure content is set even if empty
        }
    }, [editor, content]);

    useEffect(() => {
        if (editor) {
            const handleUpdate = () => {
                const updatedContent = editor.getHTML();
                onChange(updatedContent);
            };

            editor.on('update', handleUpdate);

            return () => {
                editor.off('update', handleUpdate); // Clean up event listener
            };
        }
    }, [editor, onChange]);

    return (
        <div>
            <RichTextEditor editor={editor}>
                <RichTextEditor.Toolbar sticky stickyOffset="var(--docs-header-height)">
                    <RichTextEditor.ControlsGroup>
                        <RichTextEditor.Bold />
                        <RichTextEditor.Italic />
                        <RichTextEditor.Underline />
                        <RichTextEditor.Strikethrough />
                        <RichTextEditor.ClearFormatting />
                        <RichTextEditor.Highlight />
                        <RichTextEditor.Code />
                    </RichTextEditor.ControlsGroup>

                    <RichTextEditor.ControlsGroup>
                        <RichTextEditor.H1 />
                        <RichTextEditor.H2 />
                        <RichTextEditor.H3 />
                        <RichTextEditor.H4 />
                    </RichTextEditor.ControlsGroup>

                    <RichTextEditor.ControlsGroup>
                        <RichTextEditor.Blockquote />
                        <RichTextEditor.Hr />
                        <RichTextEditor.BulletList />
                        <RichTextEditor.OrderedList />
                        <RichTextEditor.Subscript />
                        <RichTextEditor.Superscript />
                    </RichTextEditor.ControlsGroup>

                    <RichTextEditor.ControlsGroup>
                        <RichTextEditor.Link />
                        <RichTextEditor.Unlink />
                    </RichTextEditor.ControlsGroup>

                    <RichTextEditor.ControlsGroup>
                        <RichTextEditor.AlignLeft />
                        <RichTextEditor.AlignCenter />
                        <RichTextEditor.AlignJustify />
                        <RichTextEditor.AlignRight />
                    </RichTextEditor.ControlsGroup>

                    <RichTextEditor.ControlsGroup>
                        <RichTextEditor.Undo />
                        <RichTextEditor.Redo />
                    </RichTextEditor.ControlsGroup>
                </RichTextEditor.Toolbar>

                <RichTextEditor.Content />
            </RichTextEditor>
        </div>
    );
};