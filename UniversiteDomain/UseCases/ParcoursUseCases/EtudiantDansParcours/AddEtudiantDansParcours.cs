using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Add;

public class AddEtudiantDansParcours(IEtudiantRepository etudiantRepository, IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(Etudiant etudiant, Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(parcours);
        return await ExecuteAsync(etudiant.Id, parcours.Id);
    }

    public async Task<Parcours> ExecuteAsync(long idEtudiant, long idParcours)
    {
        await CheckBusinessRules(idEtudiant, idParcours);
        return await parcoursRepository.AddEtudiantAsync(idParcours, idEtudiant);
    }

    
    
    private async Task CheckBusinessRules(long idParcours, long idEtudiant)
    {
        // Vérification des paramètres
        ArgumentNullException.ThrowIfNull(idParcours);
        ArgumentNullException.ThrowIfNull(idEtudiant);
        
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idParcours);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idEtudiant);
        
        // Vérifions tout d'abord que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(etudiantRepository);
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        
        // On recherche l'étudiant
        List<Etudiant> etudiant = await etudiantRepository.FindByConditionAsync(e=>e.Id.Equals(idEtudiant));;
        if (etudiant is { Count: 0 }) throw new EtudiantNotFoundException(idEtudiant.ToString());
        // On recherche le parcours
        List<Parcours> parcours = await parcoursRepository.FindByConditionAsync(p=>p.Id.Equals(idParcours));;
        if (parcours is { Count: 0 }) throw new ParcoursNotFoundException(idParcours.ToString());
        
        // On vérifie que l'étudiant n'est pas déjà dans le parcours
        List<Etudiant> inscrit = await etudiantRepository.FindByConditionAsync(e=>e.Id.Equals(idEtudiant) && e.ParcoursSuivi.Id.Equals(idParcours));
        if (inscrit is { Count: > 0 }) throw new DuplicateInscriptionException(idEtudiant+" est déjà inscrit dans le parcours dans le parcours : "+idParcours);      
    }
}       