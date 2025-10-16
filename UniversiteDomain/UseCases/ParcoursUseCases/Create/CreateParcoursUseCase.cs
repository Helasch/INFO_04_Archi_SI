using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;


public class CreateParcoursUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(string NomParcours, int AnneeFormation)
    {
        var parcours = new Parcours{NomParcours = NomParcours, AnneeFormation = AnneeFormation};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours et = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return et;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        
        // On recherche un parcours avec le même nom
        List<Parcours> existe = await parcoursRepository.FindByConditionAsync(e=>e.NomParcours.Equals(parcours.NomParcours));
    
        // Si un parcours avec le même nom de parcours existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateNumEtudException(parcours.NomParcours+ " - ce nom de parcours est déjà affecté à un parcours");
        
        
        ArgumentOutOfRangeException.ThrowIfNegative(parcours.AnneeFormation);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(parcours.AnneeFormation, DateTime.Today.Year);
    }
}