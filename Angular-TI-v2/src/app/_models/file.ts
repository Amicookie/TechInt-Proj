export interface IFile {
    file_id: number,
    file_name: string,
    file_content: string,
    file_creation_date: Date,
    file_update_date: Date,
    file_creator_id: number,
    file_last_editor_id: number,
    user_id: number
}