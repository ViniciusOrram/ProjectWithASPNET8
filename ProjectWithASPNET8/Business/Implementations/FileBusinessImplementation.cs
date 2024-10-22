using ProjectWithASPNET8.Data.VO;

namespace ProjectWithASPNET8.Business.Implementations
{
    public class FileBusinessImplementation : IFileBusiness
    {
        private readonly string _basePath;
        private readonly IHttpContextAccessor _context;

        public FileBusinessImplementation(IHttpContextAccessor context)
        {
            _context = context;
            _basePath = Directory.GetCurrentDirectory() + "\\UploadDir\\";
        }

        public byte[] GetFile(string filename)
        {
            var filePath = _basePath + filename;
            return File.ReadAllBytes(filePath);
        }

        public async Task<FileDetailVO> SaveFileToDisk(IFormFile file)
        {
            //As informações que ele irá retornar
            FileDetailVO fileDetail = new FileDetailVO();

            //Descobrindo a extenssão do arquivo
            var fileType = Path.GetExtension(file.FileName);

            //montando a baseUrl baseando-se no Host
            var baseUrl = _context.HttpContext.Request.Host;

            //verificando qual é o tipo de extenssão
            if(fileType.ToLower() == ".pdf" || fileType.ToLower() == ".jpg" ||
               fileType.ToLower() == ".png" || fileType.ToLower() == ".jpeg")
            {
                //Pega o nome do arquivo e armazena numa variavel, setando ela no destination
                var docName = Path.GetFileName(file.FileName);

                //Se passar pela condição, começa a gravação do arquivo
                if(file != null && file.Length > 0 )
                {
                    //Setando as configurações antes de salvar em disco
                    var destination = Path.Combine(_basePath, "", docName);

                    //Setando o documentName recebendo o docName
                    fileDetail.DocumentName = docName;

                    //Setando o docType recebendo o fileType
                    fileDetail.DocType = fileType;

                    //Montando o endereço que o arquivo estara dispovivel para baixar
                    fileDetail.DocUrl = Path.Combine(baseUrl + "/api/file/v1/" + fileDetail.DocumentName);

                    //Gravando
                    using var stream = new FileStream(destination, FileMode.Create);

                    //Iniciando a gravação
                    await file.CopyToAsync(stream);
                }
            }

            return fileDetail;
        }

        public async Task<List<FileDetailVO>> SaveFilesToDisk(IList<IFormFile> files)
        {
            List<FileDetailVO> list = new List<FileDetailVO>();
            foreach (var file in files)
            {
                list.Add(await SaveFileToDisk(file));
            }
            return list;
        }


    }
}
