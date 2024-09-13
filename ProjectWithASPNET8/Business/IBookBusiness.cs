using ProjectWithASPNET8.Data.VO;
using ProjectWithASPNET8.Model;

namespace ProjectWithASPNET8.Business
{
    public interface IBookBusiness
    {
        BookVO Create(BookVO book);
        BookVO FindById(long id);
        List<BookVO> FindAll();
        BookVO Update(BookVO book);
        void Delete(long id);
    }
}
