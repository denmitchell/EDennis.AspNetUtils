In contrast to CrudService<Artist> -- which lacked the ability to 
cascade delete related Song records and thus needed to be extended, 
hence ArtistService -- CrudService<Song> had all the required 
functionality; hence, there was no need to extend that class.